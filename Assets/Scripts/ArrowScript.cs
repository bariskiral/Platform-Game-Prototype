using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private float destroyTime = 5f;

    [SerializeField] private float arrowDamage = 1f;

    private GameObject player;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        //Fade out
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.002f);
        Destroy(gameObject, destroyTime * 2);
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

            Destroy(gameObject, destroyTime);
        }

        if (col.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            playerHealth.GainHealth(arrowDamage);
            col.gameObject.GetComponent<EnemyController>().EnemyTakeDamage(arrowDamage);
            
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
