using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{

    private GameObject player;
    private GameObject enemy;
    private PlayerHealth playerHealth;
    private EnemyController enemyController;

    [SerializeField] private float currTime;
    [SerializeField] private float nextDmg;
    [SerializeField] private float damageValue;
    [SerializeField] private bool canDamageEnemy;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyController = enemy.GetComponent<EnemyController>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player && currTime <= 0)
        {
            playerHealth.TakeDamage(damageValue);
            currTime = nextDmg;
        }

        else if (col.gameObject.CompareTag("Enemy") && canDamageEnemy && currTime <= 0)
        {
            enemyController.EnemyTakeDamage(damageValue);
            currTime = nextDmg;
        }

        else
        {
            currTime -= Time.deltaTime;
        }
    }
}
