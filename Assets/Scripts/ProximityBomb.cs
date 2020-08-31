using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityBomb : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private float radius;
    [SerializeField] private float damage;

    private float countdown;
    private bool hasExploded;
    private bool trigger;
    private CircleCollider2D bombCollider;

    void Start()
    {
        bombCollider = GetComponent<CircleCollider2D>();
        countdown = timer;
        bombCollider.radius = radius;
    }

    void Update()
    {
        if (trigger)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0 && !hasExploded)
            {
                Explode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            trigger = true;
        }
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D nearbyObjects in colliders)
        {
            EnemyController enemy = nearbyObjects.GetComponent<EnemyController>();
            PlayerHealth player = nearbyObjects.GetComponent<PlayerHealth>();

            if (enemy != null)
            {
                enemy.EnemyTakeDamage(damage);
            }
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
        hasExploded = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
