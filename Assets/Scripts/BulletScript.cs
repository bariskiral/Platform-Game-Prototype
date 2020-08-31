using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth playerHealth;

    [SerializeField] private float bulletDamage = 1f;
    [SerializeField] private float destroyTime = 5f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            playerHealth.TakeDamage(bulletDamage);
        }
        else if(col.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, destroyTime);
        }
    }
}
