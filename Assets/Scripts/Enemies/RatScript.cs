using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatScript : EnemyController
{
    [Header("Rat Parameters")]
    [SerializeField] private float poisonDamage;
    [SerializeField] private float poisonDamageSec;
    [SerializeField] private int poisonEffectCount;
    [SerializeField] private float minWanderTime;
    [SerializeField] private float maxWanderTime;
    [SerializeField] private float turnTime;

    private bool poisoned;
    private bool turned;
    private float wanderTime;
    private float _turnTime;
    private float nextDamage;
    private int count;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (poisoned)
        {
            Poison();
        }
    }

    protected override void Patrol()
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

        if (turned)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);

            if (_turnTime > 0)
            {
                _turnTime -= Time.deltaTime;
                return;
            }
            _turnTime = turnTime;
        }

        if (moveRight)
        {
            if (wanderTime > 0)
            {
                rb.velocity = new Vector2(enemySpeed, rb.velocity.y);
                transform.eulerAngles = new Vector2(0, -180);
                wanderTime -= Time.deltaTime;
                turned = false;
            }
            else
            {
                wanderTime = Random.Range(minWanderTime, maxWanderTime);
                moveRight = !moveRight;
                turned = true;
            }
        }
        else
        {
            if (wanderTime > 0)
            {
                rb.velocity = new Vector2(-enemySpeed, rb.velocity.y);
                transform.eulerAngles = new Vector2(0, 0);
                wanderTime -= Time.deltaTime;
                turned = false;
            }

            else
            {
                wanderTime = Random.Range(minWanderTime, maxWanderTime);
                moveRight = !moveRight;
                turned = true;
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();

        enemyAnim.SetTrigger("Attack");
        playerRb.AddForce(transform.right * -knockPower);

        Collider2D[] hittingObj = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, damageTarget);

        foreach (Collider2D hittingCols in hittingObj)
        {
            hittingCols.GetComponent<PlayerHealth>().TakeDamage(enemyDamage);
            poisoned = true;
            count = 0;
        }

    }

    private void Poison()
    {
        if (count < poisonEffectCount)
        {
            if (nextDamage <= 0)
            {
                playerHealth.TakeDamage(poisonDamage);
                nextDamage = poisonDamageSec;
                count++;
            }
            else
            {
                nextDamage -= Time.deltaTime;
            }
        }
        else
        {
            poisoned = false;
        }
    }
}
