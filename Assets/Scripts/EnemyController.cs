using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected Animator enemyAnim;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected Transform edgeCheck;
    [SerializeField] protected Transform castPoint;
    [SerializeField] protected LayerMask whatIsObstacle;

    [SerializeField] protected float enemyHealth = 10f;
    [SerializeField] protected float enemySpeed = 2f;
    [SerializeField] protected float enemyDamage = 5f;
    [SerializeField] protected float patrolWaitTime = 2f;
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float checkRadius;
    [SerializeField] protected bool moveRight;

    protected float currEnemyHealth;
    protected float patrolPassedTime;
    protected float currTime = 0f;
    protected float nextDmg = 0.5f;
    protected bool hittingWall;
    protected bool notAtEdge;

    protected GameObject player;
    protected PlayerHealth playerHealth;
    protected Rigidbody2D rb;
    protected Transform target;

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        target = player.GetComponent<Transform>();
    }

    protected virtual void Start()
    {
        currEnemyHealth = enemyHealth;
        patrolPassedTime = patrolWaitTime;
    }

    protected virtual void FixedUpdate()
    {
        if (!CanSeePlayer(aggroRange))
        {
            Patrol();
        }

        else
        {
            FollowPlayer();
        }
    }

    protected virtual void Patrol()
    {
        hittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsObstacle);
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);

        if (hittingWall || !notAtEdge)
        {
            rb.velocity = Vector2.zero;
            enemyAnim.SetFloat("Speed", 0);

            if (patrolPassedTime > 0)
            {
                patrolPassedTime -= Time.deltaTime;
                return;
            }
            moveRight = !moveRight;
            patrolPassedTime = patrolWaitTime;
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

        enemyAnim.SetFloat("Speed", enemySpeed);
    }

    protected virtual bool CanSeePlayer(float distance)
    {
        bool seesPlayer = false;

        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, target.position, whatIsObstacle);

        if (hit.distance <= distance)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                seesPlayer = true;
                if (hit.distance <= attackRange)
                {
                    Attack();
                }
            }
            else
            {
                seesPlayer = false;
            }
            Debug.DrawLine(castPoint.position, hit.point, Color.red);
        }

        return seesPlayer;

    }

    protected virtual void FollowPlayer()
    {
        //TODO1: Little delay on turn.
        //TODO2: Edge Check.
        if (transform.position.x < target.position.x)
        {
            rb.velocity = new Vector2(enemySpeed * 1.5f, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, -180);
            moveRight = true;
        }
        else
        {
            rb.velocity = new Vector2(-enemySpeed * 1.5f, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, 0);
            moveRight = false;
        }
    }

    protected virtual void Attack()
    {
        //Different attack for every type of enemy.
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius); //Wall Check
        Gizmos.DrawWireSphere(edgeCheck.position, checkRadius); //Edge Check
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(castPoint.position, aggroRange);  //Aggro Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(castPoint.position, attackRange); //Attack Range
    }

    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (currTime <= 0)
            {
                //TODO: Little knockback to player.
                playerHealth.TakeDamage(enemyDamage);
                currTime = nextDmg;
            }
            else
            {
                currTime -= Time.deltaTime;
            }
        }
    }

    public virtual void EnemyTakeDamage(float damage)
    {

        //TODO: Little knockback to enemy.
        FollowPlayer();
        currEnemyHealth -= damage;
        enemyAnim.SetTrigger("Damage");

        if (currEnemyHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        enemyAnim.SetBool("isDead", true);
        enemySpeed = 0;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5f);
    }
}
