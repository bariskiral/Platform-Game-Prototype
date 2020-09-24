using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables

    [Header("Drag Components")]
    [SerializeField] protected Animator enemyAnim;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected Transform edgeCheck;
    [SerializeField] protected Transform castPoint;
    [SerializeField] protected Transform hitPoint;
    [SerializeField] protected LayerMask whatIsObstacle;
    [SerializeField] protected LayerMask damageTarget;
    [SerializeField] protected UIHealthBar healthBar;
    [SerializeField] protected GameObject floatingValues;

    [Header("Behaviours")]
    [SerializeField] protected bool moveRight;
    [SerializeField] protected bool touchDamage;

    [Header("Movement")]
    [SerializeField] protected float enemySpeed = 2f;
    [SerializeField] protected float aggroSpeed = 3f;
    [SerializeField] protected float patrolWaitTime = 2f;
    [SerializeField] protected float checkRadius = 0.1f;
    [SerializeField] protected float stunTime = 0.5f;
    [SerializeField] protected float getKnockedPwr = 100000f;

    [Header("Combat")]
    [SerializeField] protected float enemyHealth = 10f;
    [SerializeField] public float enemyShield = 0f;
    [SerializeField] public float enemyDamage = 5f;
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackArea = 1f;
    [SerializeField] protected float knockPower = 500f;

    [Header("Combat Timers")]
    [SerializeField] protected float attackDelay = 1f;
    [SerializeField] protected float aggroTime = 5f;
    [SerializeField] protected float touchDmgTime = 0.5f;
    [SerializeField] protected float dissappearTime = 5f;

    [HideInInspector] public float _enemyHealth;
    [HideInInspector] public float _enemyShield;

    protected float _touchDmgTime;
    protected float _patrolWaitTime;
    protected float _stunTime;
    protected float _attackDelay;
    protected float _enemySpeed;
    protected float _aggroTime;
    protected bool hittingWall;
    protected bool notAtEdge;
    protected bool seesPlayer;
    protected bool notDead = true;
    protected bool notAttacking = true;
    protected bool stunned;
    protected bool playerDashing;
    protected bool isPlayerRange;

    protected GameObject player;
    protected PlayerHealth playerHealth;
    protected Rigidbody2D rb;
    protected Rigidbody2D playerRb;
    protected Transform target;

    #endregion

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        target = player.GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        _enemyHealth = enemyHealth;
        _enemyShield = enemyShield;
        _enemySpeed = enemySpeed;
        _patrolWaitTime = patrolWaitTime;
        _stunTime = stunTime;
        healthBar.SetHealth(_enemyHealth, enemyHealth);
        healthBar.SetShield(_enemyShield, enemyShield);
    }

    protected virtual void FixedUpdate()
    {
        if (!CanSeePlayer(aggroRange) && notDead)
        {
            Patrol();
        }

        else
        {
            FollowPlayer();
        }

        if (stunned)
        {
            rb.velocity = Vector2.zero;
            _stunTime -= Time.deltaTime;
            if (_stunTime <= 0)
            {
                stunned = false;
                _stunTime = stunTime;
            }
        }

        enemyAnim.SetFloat("Speed", Math.Abs(rb.velocity.x));
        playerDashing = player.GetComponent<PlayerController>().isDashing;
    }

    protected virtual void Patrol()
    {
        hittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsObstacle);
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);

        if (_aggroTime <= 0)
        {
            enemySpeed = _enemySpeed;
        }

        _aggroTime -= Time.deltaTime;

        if (hittingWall || !notAtEdge)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (_patrolWaitTime > 0)
            {
                _patrolWaitTime -= Time.deltaTime;
                return;
            }
            moveRight = !moveRight;
            _patrolWaitTime = patrolWaitTime;
        }

        if (moveRight)
        {
            rb.velocity = new Vector2(enemySpeed, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, -180);
        }
        else
        {
            rb.velocity = new Vector2(-enemySpeed, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, 0);
        }
    }

    protected virtual bool CanSeePlayer(float distance)
    {
        seesPlayer = false;

        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, target.position, whatIsObstacle);

        if (hit.distance <= distance)
        {
            if (hit.collider.gameObject == player)
            {
                seesPlayer = true;

                if (hit.distance <= attackRange)
                {
                    if (Time.time >= _attackDelay && !stunned && notDead && !playerDashing)
                    {
                        notAttacking = false;
                        isPlayerRange = true;
                        enemyAnim.SetTrigger("Attack");
                        _attackDelay = Time.time + attackDelay;
                    }
                }
                else
                {
                    isPlayerRange = false;
                }
            }

            Debug.DrawLine(castPoint.position, hit.point, Color.red);
        }

        return seesPlayer;
    }

    protected virtual void FollowPlayer()
    {
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);
        _aggroTime = aggroTime;
        enemySpeed = aggroSpeed;

        if (notAtEdge && notDead && notAttacking && !isPlayerRange)
        {
            if (transform.position.x < target.position.x)
            {
                rb.velocity = new Vector2(aggroSpeed, rb.velocity.y);
                transform.eulerAngles = new Vector2(0, -180);
                moveRight = true;
            }
            else
            {
                rb.velocity = new Vector2(-aggroSpeed, rb.velocity.y);
                transform.eulerAngles = new Vector2(0, 0);
                moveRight = false;
            }
        }
        else
        {
            //FIX: If player comes from back when enemy is waiting in the edge, it will stop until player leaves range.
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    protected virtual void NotAttackingTrigger()
    {
        notAttacking = true;
    }

    protected virtual void Attack()
    {
        //Different attack for every type of enemy.
    }

    public virtual void EnemyTakeDamage(float damage)
    {
        rb.AddForce(transform.right * getKnockedPwr);

        if (_enemyShield > 0)
        {
            _enemyShield -= damage;
            healthBar.SetShield(_enemyShield, enemyShield);
        }
        else
        {
            _enemyHealth -= damage;
            healthBar.SetHealth(_enemyHealth, enemyHealth);
        }

        GameObject damageDisplay = Instantiate(floatingValues, transform.position, Quaternion.identity);
        damageDisplay.transform.SetParent(transform);
        damageDisplay.transform.GetChild(0).GetComponent<TextMesh>().text = "" + damage;

        notAttacking = true;
        enemyAnim.SetTrigger("Damage");
        _stunTime = stunTime;
        stunned = true;
        _attackDelay = Time.time + attackDelay;
        //FIX: If enemy takes damage from traps, it will still follow player.
        FollowPlayer();

        if (_enemyHealth <= 0)
        {
            healthBar.enemyDied(true);
            Die();
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (touchDamage)
        {
            if (col.gameObject == player)
            {
                if (_touchDmgTime <= 0)
                {
                    playerHealth.TakeDamage(enemyDamage);
                    _touchDmgTime = touchDmgTime;
                }
                else
                {
                    _touchDmgTime -= Time.deltaTime;
                }
            }
        }
    }

    protected virtual void Die()
    {
        notDead = false;
        enemyAnim.SetBool("isDead", true);
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, dissappearTime);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        Gizmos.DrawWireSphere(edgeCheck.position, checkRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(castPoint.position, aggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(castPoint.position, attackRange);
        Gizmos.DrawWireSphere(hitPoint.position, attackArea);
    }
}
