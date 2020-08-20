using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatScript : EnemyController
{
    private Vector2 moveDir;
    private Vector2 movePerSec;
    private float timePassed;
    private bool canRoam = false;

    [SerializeField] private float dirChangeTime = 2f;
    
    protected override void Start()
    {
        base.Start();
        timePassed = 0f;
        calcuateNewMovementVector();
    }

    protected override void Update()
    {
        if (canRoam)
        {
            enemyRoam();
        }
    }

    private void calcuateNewMovementVector()
    {
        moveDir = new Vector2(Random.Range(-1.0f, 1.0f), 0).normalized;
        movePerSec = moveDir * enemySpeed;
    }

    private void enemyRoam()
    {
        if (Time.time - timePassed > dirChangeTime)
        {
            timePassed = Time.time;         
        }

        transform.position = new Vector2(transform.position.x + (movePerSec.x * Time.deltaTime),
        transform.position.y + (movePerSec.y * Time.deltaTime));
    }

    protected override void Die()
    {
        canRoam = false;
        base.Die();
    }
}
