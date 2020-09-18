using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] private float arrowDamage = 1f;
    [SerializeField] private float destroyTime = 5f;

    private bool fadeOut;

    private GameObject player;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (fadeOut)
        {
            GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.002f);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            GameObject sharedParent = new GameObject("ArrowHole");
            sharedParent.transform.position = col.transform.position;
            sharedParent.transform.rotation = col.transform.rotation;

            gameObject.GetComponent<Rigidbody2D>().isKinematic = true;

            sharedParent.transform.parent = col.gameObject.transform;
            transform.parent = sharedParent.transform;
            fadeOut = true;
            Destroy(gameObject, destroyTime);
        }

        else if (col.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

            if (col.gameObject.GetComponent<EnemyController>()._enemyShield <= 0)
            {
                playerHealth.GainHealth(arrowDamage);
            }    
            col.gameObject.GetComponent<EnemyController>().EnemyTakeDamage(arrowDamage);
        }

        else
        {
            Destroy(gameObject, destroyTime * 2);
        }
    }

    private void OnDestroy()
    {
        if(transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }

}
