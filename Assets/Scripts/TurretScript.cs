using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform target;
    [SerializeField] private Transform rotatingObject;
    [SerializeField] private LayerMask whatIsObstacle;

    [SerializeField] private float shotRange;
    [SerializeField] private float shotDelay;
    [SerializeField] private float shotForce = 500f;

    private GameObject player;
    private float shotTime;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (CanSeePlayer(shotRange))
        {
            //TODO: Aggro anim.
            Shoot();
            Rotate();
        }
        else
        {
            //TODO: Idle anim.
            rotatingObject.rotation = Quaternion.Euler(0, 0, 0);
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
            GameObject bulletClone = Instantiate(bullet, castPoint.position, Quaternion.identity);
            Vector2 dir = (player.transform.position - castPoint.position).normalized;
            bulletClone.GetComponent<Rigidbody2D>().AddForce(dir * shotForce);

            if (transform.position.x < target.position.x)
            {
                bulletClone.GetComponent<BulletScript>().bulletDir = -1;
            }
            else
            {
                bulletClone.GetComponent<BulletScript>().bulletDir = 1;
            }

            shotTime = 0;
        }
    }

    private void Rotate()
    {
        Vector2 dir = (player.transform.position - rotatingObject.position).normalized;
        rotatingObject.transform.right = dir;
    }
}
