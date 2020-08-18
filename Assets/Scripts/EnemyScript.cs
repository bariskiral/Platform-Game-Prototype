using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private float maxHealth = 10f;

    private float currHealth;

    void Start()
    {
        currHealth = maxHealth;
    }

    public void EnemyTakeDamage(float damage)
    {
        currHealth -= damage;
        anim.SetTrigger("Damage");

        if (currHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        anim.SetBool("isDead", true);

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 5f);
    }

}
