using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWithArc : MonoBehaviour
{
    [SerializeField] private float throwSpeed;
    [SerializeField] private float proWithArcDmg;
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private float knockPower = 500f;
    [SerializeField] private Vector3 Offset;

    private GameObject player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRb;
    private bool playerDashing;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Vector2 direction = -transform.right + Offset;
        GetComponent<Rigidbody2D>().AddForce(direction * throwSpeed, ForceMode2D.Impulse);
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        playerDashing = player.GetComponent<PlayerController>().isDashing;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject == player && !playerDashing)
        {
            Destroy(gameObject);
            playerHealth.TakeDamage(proWithArcDmg);
            playerRb.AddForce(transform.right * -knockPower);
        }
        else if (col.gameObject.CompareTag("Ground"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(gameObject, destroyTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
