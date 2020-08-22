using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatScript : EnemyController
{
    private bool canRoam = true;

    [SerializeField] private float dirChangeTime = 2f;
    private float timeLeft;
    private Vector2 movement;

    protected override void Start()
    {
        base.Start();
        timeLeft = 0f;
    }

    protected override void FixedUpdate()
    {
        if (canRoam)
        {
            EnemyRoam();
        }
    }

    private void EnemyRoam()
    {
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            movement = new Vector2(Random.Range(-1f, 1f), 0).normalized;
            timeLeft += dirChangeTime;
        }

        hittingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsObstacle);
        notAtEdge = Physics2D.OverlapCircle(edgeCheck.position, checkRadius, whatIsObstacle);

        if (hittingWall || !notAtEdge)
        {
            moveRight = !moveRight;
        }

        if (moveRight)
        {
            rb.velocity = new Vector2(movement.x * enemySpeed, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, -180);
        }
        else
        {
            rb.velocity = new Vector2(-movement.x * enemySpeed, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, 0);
        }

        enemyAnim.SetFloat("Speed", enemySpeed);
    }
}
