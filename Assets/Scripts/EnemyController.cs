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
    [SerializeField] protected float agroRange = 5f;
    [SerializeField] protected float checkRadius;
    [SerializeField] protected bool moveRight;

    protected float currEnemyHealth;
    protected float currTime = 0f;
    protected float nextDmg = 0.5f;
    protected float patrolPassedTime;
    protected float castDistance;
    protected bool hittingWall;
    protected bool notAtEdge;
    protected bool isAggro;

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
        if (!CanSeePlayer(agroRange))
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
        castDistance = distance;

        if (moveRight)
        {
            castDistance = -distance;
        }

        Vector2 endPos = castPoint.position + Vector3.left * castDistance;
        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, endPos, whatIsObstacle);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                seesPlayer = true;
            }
            else
            {
                seesPlayer = false;
            }

            Debug.DrawLine(castPoint.position, hit.point, Color.yellow);
        }
        else
        {
            Debug.DrawLine(castPoint.position, endPos, Color.blue);
        }

        return seesPlayer;
    }

    protected virtual void FollowPlayer()
    {
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

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        Gizmos.DrawWireSphere(edgeCheck.position, checkRadius);
    }

    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (currTime <= 0)
            {
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
