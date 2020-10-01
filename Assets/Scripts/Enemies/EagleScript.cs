using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleScript : EnemyController
{
    private bool flyingEnemyMoving;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
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

        if (flyingEnemyMoving)
        {
            enemyAnim.SetFloat("Speed", 1);
        }
        else
        {
            enemyAnim.SetFloat("Speed", 0);
        }
        
        playerDashing = player.GetComponent<PlayerController>().isDashing;
    }

    protected override void Patrol()
    {
        enemySpeed = _enemySpeed;
        flyingEnemyMoving = false;
    }

    protected override bool CanSeePlayer(float distance)
    {
        return base.CanSeePlayer(distance);
    }

    protected override void FollowPlayer()
    {

        //_aggroTime = aggroTime;
        enemySpeed = aggroSpeed;

        if (notDead && notAttacking && !isPlayerRange)
        {
            if (transform.position.x < target.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, aggroSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector2(0, -180);
                moveRight = true;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, aggroSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector2(0, 0);
                moveRight = false;
            }
            flyingEnemyMoving = true;
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    protected override void Attack()
    {
        base.Attack();
        Collider2D[] hittingObj = Physics2D.OverlapCircleAll(hitPoint.position, attackArea, damageTarget);

        foreach (Collider2D hittingCols in hittingObj)
        {
            hittingCols.GetComponent<PlayerHealth>().TakeDamage(enemyDamage);
            playerRb.AddForce(transform.right * -knockPower);
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(castPoint.position, aggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(castPoint.position, attackRange);
    }
}
