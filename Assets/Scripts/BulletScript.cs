using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private GameObject player;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;

    [SerializeField] private float bulletDamage = 1;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Vector2 dir = (player.transform.position - transform.position).normalized;
        rb.AddForce(dir*1000);
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
            Destroy(gameObject, 5);
        }
    }
}
