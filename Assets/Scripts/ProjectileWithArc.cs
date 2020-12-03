using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWithArc : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private float expRadius;
    [SerializeField] private float throwSpeed;
    [SerializeField] private float proWithArcDmg;
    [SerializeField] private float destroyTime = 5f;
    [SerializeField] private float knockPower = 500f;
    [SerializeField] private Vector3 Offset;

    private GameObject player;
    private PlayerHealth playerHealth;
    private ProjectileWithArc projectileWithArc;
    private Rigidbody2D playerRb;

    private float countdown;
    private bool playerDashing;
    private bool trigger;
    private bool hasExploded;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        countdown = timer;
        Vector2 direction = -transform.right + Offset;
        GetComponent<Rigidbody2D>().AddForce(direction * throwSpeed, ForceMode2D.Impulse);
    }

    private void Update()
    {
        playerDashing = player.GetComponent<PlayerController>().isDashing;

        if (trigger)
        {
            countdown -= Time.deltaTime;

            if (countdown <= 0 && !hasExploded)
            {
                Explode();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject == player && !playerDashing || col.gameObject.CompareTag("PlayerProjectile"))
        {
            Explode();            
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            trigger = true;
        }
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, expRadius);
        foreach (Collider2D nearbyObjects in colliders)
        {
            PlayerHealth playerInRange = nearbyObjects.GetComponent<PlayerHealth>();

            if (playerInRange != null)
            {
                playerInRange.TakeDamage(proWithArcDmg);
                playerRb.AddForce(transform.right * -knockPower);
            }
        }
        hasExploded = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, expRadius);
    }

}
