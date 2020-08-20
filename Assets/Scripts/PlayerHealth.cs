using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float currHealth;
    [SerializeField] private TextMeshProUGUI healthText;

    private GameObject player;
    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = player.GetComponent<Animator>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (currHealth >= 0)
        {
            healthText.text = "HP: " + currHealth;
        }
        else
        {
            healthText.text = "HP: 0";
        }
    }

    public void TakeDamage(float dmgAmount)
    {
        currHealth -= dmgAmount;
        player.GetComponent<Animation>().Play("Damaged");
        if (currHealth <= 0)
        {
            Died();
        }
    }

    public void GainHealth(float hpAmount)
    {
        currHealth += hpAmount;
    }

    private void Died()
    {
        player.GetComponent<Animation>().Stop("Damaged");
        anim.SetBool("isDead", true);
        rb.velocity = Vector2.zero;
        player.GetComponent<PlayerController>().enabled = false;
    }

}
