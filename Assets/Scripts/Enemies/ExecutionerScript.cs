using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerScript : EnemyController
{
    [Header("Executioner Parameters")]
    [SerializeField] private GameObject bomb;
    [SerializeField] private float bombDelay;
    [SerializeField] private float bombAttackRange;

    private float _bombDelay;

    private void Update()
    {
        Rotate();
    }
    protected override bool CanSeePlayer(float distance)
    {
        bool seesPlayer = false;
        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, target.position, whatIsObstacle);

        if (hit.distance <= distance)
        {
            if (hit.collider.gameObject == player)
            {
                seesPlayer = true;

                if (hit.distance <= attackRange)
                {
                    notAttacking = false;

                    if (Time.time >= _attackDelay && !stunned && notDead)
                    {
                        //Attack();
                        enemyAnim.SetTrigger("Attack");
                        _attackDelay = Time.time + attackDelay;
                    }
                }

                else if (hit.distance > attackRange && hit.distance <= bombAttackRange)
                {
                    notAttacking = false;

                    if (Time.time >= _bombDelay && !stunned && notDead)
                    {
                        ThrowBomb();
                        _bombDelay = Time.time + bombDelay;
                    }
                }

                else
                {
                    notAttacking = true;
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

    protected override void Attack()
    {
        base.Attack();
        //enemyAnim.SetTrigger("Attack");
        Collider2D[] hittingObj = Physics2D.OverlapCircleAll(hitPoint.position, attackArea, damageTarget);

        foreach (Collider2D hittingCols in hittingObj)
        {
            hittingCols.GetComponent<PlayerHealth>().TakeDamage(enemyDamage);
            playerRb.AddForce(transform.right * -knockPower);
        }
    }

    private void ThrowBomb()
    {
        Instantiate(bomb, hitPoint.position, transform.rotation);
    }

    private void Rotate()
    {
        if (CanSeePlayer(aggroRange))
        {
            if (transform.position.x < target.position.x)
            {
                transform.eulerAngles = new Vector2(0, -180);
            }
            else
            {
                transform.eulerAngles = new Vector2(0, 0);
            }
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.DrawWireSphere(castPoint.position, bombAttackRange);
    }

}
