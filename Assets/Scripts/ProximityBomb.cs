using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityBomb : MonoBehaviour
{
    [SerializeField] private float timer;
    [SerializeField] private float expRadius;
    [SerializeField] private float trigRadius;
    [SerializeField] private float damage;
    [SerializeField] private bool canDamageEnemy = true;
    [SerializeField] private LayerMask trigLayer;
    [SerializeField] private GameObject floatingValues;

    private float countdown;
    private bool hasExploded;
    private bool trigger;
    private GameObject countDownText;

    private void Start()
    {
        countdown = timer;
        countDownText = Instantiate(floatingValues, transform.position, Quaternion.identity);
        countDownText.transform.SetParent(transform);
    }

    private void Update()
    {
        CheckTrigger();

        //TODO: Explosion countdown effect.
        if (trigger)
        {
            countDownText.SetActive(true);
            countDownText.transform.GetChild(0).GetComponent<TextMesh>().text = "" + ((int)countdown + 1);

            countdown -= Time.deltaTime;

            if (countdown <= 0 && !hasExploded)
            {
                Explode();
            }
        }
    }

    private void CheckTrigger()
    {
        Collider2D triggerActive = Physics2D.OverlapCircle(transform.position, trigRadius, trigLayer);

        if (triggerActive != null)
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, trigRadius);
    }
}
