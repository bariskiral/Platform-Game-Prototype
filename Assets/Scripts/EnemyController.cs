using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected Animator enemyAnim;
    [SerializeField] protected float enemyHealth = 10f;
    [SerializeField] protected float enemySpeed = 2f;
    [SerializeField] protected float enemyDamage = 5f;

    protected float currEnemyHealth;
    protected float currTime = 0f;
    protected float nextDmg = 0.5f;

    protected GameObject player;
    protected PlayerHealth playerHealth;


    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    protected virtual void Start()
    {
        currEnemyHealth = enemyHealth;
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (currTime <= 0)
            {
                playerHealth.TakeDamage(enemyDamage);
                currTime = nextDmg;
            }
            else
            {
                currTime -= Time.deltaTime;
            }
        }
    }

    protected virtual void followPlayer()
    {

    }

    public virtual void EnemyTakeDamage(float damage)
    {
        currEnemyHealth -= damage;
        enemyAnim.SetTrigger("Damage");

        if (currEnemyHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        enemyAnim.SetBool("isDead", true);
        //TODO: Make enemies explode?
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5f);
    }
}
