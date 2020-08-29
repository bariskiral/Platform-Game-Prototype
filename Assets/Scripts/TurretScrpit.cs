using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScrpit : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask whatIsObstacle;
    [SerializeField] private float attackRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float shotDelay;
    private float shotTime;

    private void Start()
    {
        
    }
    void Update()
    {
        if (CanSeePlayer(aggroRange))
        {
            Shoot();
        }
    }

    private bool CanSeePlayer(float distance)
    {
        bool seesPlayer = false;

        RaycastHit2D hit = Physics2D.Linecast(castPoint.position, target.position, whatIsObstacle);

        if (hit.distance <= distance)
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                seesPlayer = true;
            }
            else
            {
                seesPlayer = false;
            }
            Debug.DrawLine(castPoint.position, hit.point, Color.red);
        }

        return seesPlayer;

    }

    private void Shoot()
    {
        shotTime += Time.deltaTime;
        if (shotTime >= shotDelay)
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
            shotTime = 0;
        }
    }
}
