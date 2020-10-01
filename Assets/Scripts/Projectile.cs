using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private float knockPower = 500f;

    public float projectileDamage = 1f;
    public float projectileDir = 1;
    private bool playerDashing;

    private GameObject player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>(); 
    }

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        playerDashing = player.GetComponent<PlayerController>().isDashing;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == player && !playerDashing)
        {
            Destroy(gameObject);
            playerHealth.TakeDamage(projectileDamage);
            playerRb.AddForce(transform.right * -knockPower * projectileDir);
        }
        else if(col.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
