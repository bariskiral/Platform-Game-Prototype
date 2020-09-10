using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRb;
    public float bulletDir = 1;
    public float bulletDamage = 1f;

    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private float knockPower = 500f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            playerHealth.TakeDamage(bulletDamage);
            playerRb.AddForce(transform.right * -knockPower * bulletDir);
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
