using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolKing : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float shotForce;
    [SerializeField] private float enemyTouchDamage;
    [SerializeField] private float enemyMeleeDamage;
    [SerializeField] private float enemyRangedDamage;
    [SerializeField] private float touchDmgTime;
    [SerializeField] private Vector2 attackArea;
    [SerializeField] private Transform hitPoint;
    [SerializeField] private Transform castPoint1;
    [SerializeField] private Transform castPoint2;
    [SerializeField] private LayerMask damageTarget;
    [SerializeField] private GameObject attackWave;
    private float _touchDmgTime;
    private Rigidbody2D rb;
    private GameObject player;
    private PlayerHealth playerHealth;
    private Transform target;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        target = player.GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {

    }

    private void Update()
    {
        FK_WaveAttack();
    }

    public void FK_Follow()
    {
        if (transform.position.x < target.position.x)
        {
            rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, -180);
        }
        else
        {
            rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
            transform.eulerAngles = new Vector2(0, 0);
        }
    }

    public void FK_MeleeAttack()
    {
        Collider2D[] hittingObj = Physics2D.OverlapBoxAll(hitPoint.position, attackArea, 0, damageTarget);

        foreach (Collider2D hittingCols in hittingObj)
        {
            hittingCols.GetComponent<PlayerHealth>().TakeDamage(enemyMeleeDamage);
        }
    }

    public void FK_WaveAttack()
    {
        GameObject rightWave = Instantiate(attackWave, castPoint1.position, castPoint1.rotation);
        GameObject leftWave = Instantiate(attackWave, castPoint2.position, castPoint2.rotation);
        rightWave.GetComponent<Rigidbody2D>().AddForce(transform.right * shotForce);
        leftWave.GetComponent<Rigidbody2D>().AddForce(-transform.right * shotForce);
    }

    public void FK_Spawn()
    {
        //Spawns ads randomly
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
        //Gizmos.DrawCube(hitPoint.position, attackArea);
    }

}
