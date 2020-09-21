using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherScript : EnemyController
{
    [Header("Archer Parameters")]
    [SerializeField] private GameObject arrow;
    [SerializeField] private Transform rotatingObject;
    [SerializeField] private float shotForce = 500f;

    protected override void Start()
    {
        base.Start();
        arrow.GetComponent<Projectile>().projectileDamage = enemyDamage;
    }

    private void Update()
    {
        Rotate();
    }

    protected override void Attack()
    {
        base.Attack();
        Shoot();   
    }

    private void Shoot()
    {
        GameObject bulletClone = Instantiate(arrow, hitPoint.position, hitPoint.rotation);
        Vector2 dir = (player.transform.position - castPoint.position).normalized;
        bulletClone.GetComponent<Rigidbody2D>().AddForce(dir * shotForce);
        bulletClone.GetComponent<Projectile>().projectileDir = -1;
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

        Vector2 dir = (player.transform.position - rotatingObject.position).normalized;
        rotatingObject.transform.right = dir;
    }
}
