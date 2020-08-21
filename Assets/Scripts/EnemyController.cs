using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected Animator enemyAnim;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected Transform edgeCheck;
    [SerializeField] protected LayerMask whatIsObstacle;

    [SerializeField] protected float enemyHealth = 10f;
    [SerializeField] protected float enemySpeed = 2f;
    [SerializeField] protected float enemyDamage = 5f;
    [SerializeField] protected float waitTime = 2f;
    [SerializeField] protected float checkRadius;
    [SerializeField] protected bool moveRight;

    protected float currEnemyHealth;
    protected float currTime = 0f;
    protected float nextDmg = 0.5f;
    protected bool hittingWall;
    protected bool notAtEdge;

    protected GameObject player;
    protected PlayerHealth playerHealth;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        currEnemyHealth = enemyHealth;
    }

    protected virtual void FixedUpdate()
    {
        Patrol();
        //TODO: FollowPlayer();
    }

    protected virtual void Patrol()
    {
        hittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsObstacle);
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);

        if (hittingWall || !notAtEdge)
        {
            moveRight = !moveRight;
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

    protected virtual void FollowPlayer()
    {
        //TODO
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
        //TODO: Make enemies explode?
        enemySpeed = 0;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5f);
    }
}
