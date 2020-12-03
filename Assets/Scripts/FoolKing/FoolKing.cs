using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolKing : MonoBehaviour
{
    [Header("Drag Components")]
    [SerializeField] private LayerMask damageTarget;
    [SerializeField] private LayerMask whatIsObstacle;
    [SerializeField] private GameObject attackWave;
    [SerializeField] private GameObject spawnedEnemyType;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform castPointR;
    [SerializeField] private Transform castPointL;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float walkSpeedEnrage;
    [SerializeField] private float idleDelay;
    [SerializeField] private bool enraged;

    [Header("Combat")]
    [SerializeField] private float enemyMeleeDamage;
    [SerializeField] private float enemyRangedDamage;
    [SerializeField] private float shotForce;
    [SerializeField] private float shotForceEnrage;
    [SerializeField] private float enemyTouchDamage;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackArea;

    [Header("Combat Timers")]
    [SerializeField] private float meleeDelay;
    [SerializeField] private float meleeDelayEnrage;
    [SerializeField] private float touchDmgTime;

    [Header("Spawn Behaviour")]
    [SerializeField] private int spawnedEnemyCount;
    [SerializeField] private int spawnedEnemyCountEnrage;

    private bool notAttacking;
    private float _touchDmgTime;
    private float _meleeDelay;
    private float _idleDelay;

    private Animator foolKingAnim;
    private Rigidbody2D foolKingRb;
    private GameObject player;
    private PlayerHealth playerHealth;
    private Transform target;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        target = player.GetComponent<Transform>();
        foolKingRb = GetComponent<Rigidbody2D>();
        foolKingAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        StateChanger();
        EnrageUpdate();
    }

    private void FixedUpdate()
    {
        foolKingAnim.SetFloat("Speed", System.Math.Abs(foolKingRb.velocity.x));
        foolKingAnim.SetBool("Enraged", enraged);
    }


    private void StateChanger()
    {
        // idle - follow - melee - ranged - spawn
        if (_idleDelay <= 0)
        {
            _idleDelay = idleDelay;

            int rnd = Random.Range(0, 3);

            if (rnd == 0)
            {
                foolKingAnim.SetTrigger("Follow");
                Debug.Log("0");
            }
            else if (rnd == 1)
            {
                foolKingAnim.SetTrigger("DamageWall");
                Debug.Log("1");
            }
            else if (rnd == 2)
            {
                foolKingAnim.SetTrigger("Spawn");
                Debug.Log("2");
            }
        }
        else
        {
            _idleDelay -= Time.deltaTime;
        }
    }

    public void FK_Idle()
    {
        foolKingRb.velocity = new Vector2(0, foolKingRb.velocity.y);
    }

    public void FK_Follow()
    {
        RaycastHit2D hit = Physics2D.Linecast(hitPoint.position, target.position, whatIsObstacle);
        Debug.DrawLine(hitPoint.position, hit.point, Color.red);

        if (hit.distance <= attackDistance)
        {
            notAttacking = false;
            foolKingRb.velocity = new Vector2(0, foolKingRb.velocity.y);
            foolKingAnim.SetTrigger("MeleeAttack");
        }

        else if (notAttacking)
        {
            if (transform.position.x < target.position.x)
            {
                foolKingRb.velocity = new Vector2(walkSpeed, foolKingRb.velocity.y);
                transform.eulerAngles = new Vector2(0, 0);
            }
            else
            {
                foolKingRb.velocity = new Vector2(-walkSpeed, foolKingRb.velocity.y);
                transform.eulerAngles = new Vector2(0, -180);
            }
        }

        else
        {
            notAttacking = true;
        }
    }

    private void FK_MeleeAttack()
    {
        Collider2D[] hittingObj = Physics2D.OverlapCircleAll(hitPoint.position, attackArea, damageTarget);

        if (Time.time >= _meleeDelay)
        {
            foreach (Collider2D hittingCols in hittingObj)
            {
                hittingCols.GetComponent<PlayerHealth>().TakeDamage(enemyMeleeDamage);
                _meleeDelay = Time.time + meleeDelay;
            }
        }
    }

    public void FK_DamageWall()
    {
        GameObject rightWave = Instantiate(attackWave, castPointR.position, castPointR.rotation);
        GameObject leftWave = Instantiate(attackWave, castPointL.position, castPointL.rotation);
        rightWave.GetComponent<Rigidbody2D>().AddForce(transform.right * shotForce);
        leftWave.GetComponent<Rigidbody2D>().AddForce(-transform.right * shotForce);
    }

    private void FK_Spawn()
    {
        for (int _spawnCount = 0; _spawnCount < spawnedEnemyCount; _spawnCount++)
        {
            Instantiate(spawnedEnemyType, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        }
    }

    private void EnrageUpdate()
    {
        if (enraged) //hp<=%50
        {
            enraged = true;
            walkSpeed = walkSpeedEnrage;
            spawnedEnemyCount = spawnedEnemyCountEnrage;
            meleeDelay = meleeDelayEnrage;
            shotForce = shotForceEnrage;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject == player)
        {
            if (_touchDmgTime <= 0)
            {
                playerHealth.TakeDamage(enemyTouchDamage);
                _touchDmgTime = touchDmgTime;
            }
            else
            {
                _touchDmgTime -= Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hitPoint.position, attackArea);
    }

}
