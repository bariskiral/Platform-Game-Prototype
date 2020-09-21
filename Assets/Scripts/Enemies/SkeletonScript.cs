using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : EnemyController
{
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
}
