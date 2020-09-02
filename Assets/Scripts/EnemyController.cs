﻿using System;
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
    [SerializeField] protected float aggroSpeed = 3f;
    [SerializeField] protected float enemyDamage = 5f;
    [SerializeField] protected float patrolWaitTime = 2f;
    [SerializeField] protected float aggroRange = 5f;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float nextDmg = 0.5f;
    [SerializeField] protected float checkRadius;
    [SerializeField] protected float dissappearTime = 5f;
    [SerializeField] protected float knockPower = 200000f;
    [SerializeField] protected float stunTime = 0.5f;
    [SerializeField] protected bool moveRight;
    [SerializeField] protected bool touchDamage;
    
    protected float currEnemyHealth;
    protected float _patrolWaitTime;
    protected float currTime = 0f;
    protected float _stunTime;
    protected bool hittingWall;
    protected bool notAtEdge;
    protected bool notDead = true;
    protected bool stunned;

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
        _patrolWaitTime = patrolWaitTime;
        _stunTime = stunTime;
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
    }

    protected virtual void Patrol()
    {
        hittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsObstacle);
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);

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
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);

        if (notAtEdge && notDead)
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

    protected virtual void Attack()
    {
        //Different attack for every type of enemy.
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
    }

    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (touchDamage)
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
    }

    public virtual void EnemyTakeDamage(float damage)
    {
        rb.AddForce(transform.right * knockPower);
        currEnemyHealth -= damage;
        enemyAnim.SetTrigger("Damage");
        stunned = true;
        //FIX: If enemy takes damage from traps, it will still follow player.
        FollowPlayer();

        if (currEnemyHealth <= 0)
        {
            Die();
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
}
