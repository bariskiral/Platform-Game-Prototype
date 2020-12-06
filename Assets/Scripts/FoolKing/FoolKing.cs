using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolKing : MonoBehaviour
{
    #region Variables

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

    [Header("Behaviours")]
    [SerializeField] private bool following;
    [SerializeField] private bool idle;
    [SerializeField] private bool meleeAttack;
    [SerializeField] private bool damageWall;
    [SerializeField] private bool spawn;
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

    private bool playerDashing;
    private float _touchDmgTime;
    private float _meleeDelay;
    private int rnd;

    private Animator foolKingAnim;
    private Rigidbody2D foolKingRb;
    private GameObject player;
    private PlayerHealth playerHealth;
    private Transform target;

    #endregion

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        target = player.GetComponent<Transform>();
        foolKingRb = GetComponent<Rigidbody2D>();
        foolKingAnim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        FK_PlayerInRange();
        FK_EnrageUpdate();

        playerDashing = player.GetComponent<PlayerController>().isDashing;
        foolKingAnim.SetBool("Enraged", enraged);
        foolKingAnim.SetBool("MeleeAttack", meleeAttack);
        foolKingAnim.SetBool("Follow", following);
    }

    private void FK_StateChanger()
    {
        rnd = Random.Range(0, 3);

        following = false;
        damageWall = false;
        spawn = false;
        idle = false;

        if (rnd == 0 && !meleeAttack)
        {
            following = true;
        }

        else if (rnd == 1 && !meleeAttack)
        {
            damageWall = true;
            foolKingAnim.SetTrigger("DamageWall");
        }

        else if (rnd == 2 && !meleeAttack)
        {
            spawn = true;
            foolKingAnim.SetTrigger("Spawn");
        }

        else
        {
            idle = true;
        }
        Debug.Log("Yeni Sayi: " + rnd);
    }

    public void FK_Idle()
    {
        foolKingRb.velocity = new Vector2(0, foolKingRb.velocity.y);
    }

    private void FK_PlayerInRange()
    {
        RaycastHit2D hit = Physics2D.Linecast(hitPoint.position, target.position, whatIsObstacle);
        Debug.DrawLine(hitPoint.position, hit.point, Color.red);

        if (hit.distance <= attackDistance && !playerDashing)
        {
            foolKingRb.velocity = new Vector2(0, foolKingRb.velocity.y);
            meleeAttack = true;
        }
        else
        {
            meleeAttack = false;
        }
    }

    public void FK_Follow()
    {
        if (!meleeAttack)
        {
            following = true;
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
            following = false;
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

    private void FK_EnrageUpdate()
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
