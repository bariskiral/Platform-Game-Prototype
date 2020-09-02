using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityBomb : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private float expRadius;
    [SerializeField] private float damage;
    [SerializeField] private bool canDamageEnemy = true;

    private float countdown;
    private bool hasExploded;
    private bool trigger;

    void Start()
    {
        countdown = timer;
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

    //TODO: Explosion effect.
    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, expRadius);
        foreach (Collider2D nearbyObjects in colliders)
        {
            EnemyController enemy = nearbyObjects.GetComponent<EnemyController>();
            PlayerHealth player = nearbyObjects.GetComponent<PlayerHealth>();

            if (enemy != null && canDamageEnemy)
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
        Gizmos.DrawWireSphere(transform.position, expRadius);
    }
}
