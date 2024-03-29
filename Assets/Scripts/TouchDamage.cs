﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRb;

    [SerializeField] private float currTime;
    [SerializeField] private float nextDmg;
    [SerializeField] private float damageValue;
    [SerializeField] private float knockPower;
    [SerializeField] private bool canDamageEnemy;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player && currTime <= 0)
        {
            playerHealth.TakeDamage(damageValue);
            playerRb.AddForce(transform.up * knockPower);
            currTime = nextDmg;
        }

        else if (col.gameObject.CompareTag("Enemy") && canDamageEnemy && currTime <= 0)
        {
            col.gameObject.GetComponent<EnemyController>().EnemyTakeDamage(damageValue);
            currTime = nextDmg;
        }

        else
        {
            currTime -= Time.deltaTime;
        }
    }
}
