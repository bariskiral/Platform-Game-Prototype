using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currHealth;
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
        //Debug.Log(currHealth);
    }

    public void takeDamage(int dmgAmount)
    {
        currHealth -= dmgAmount;
        player.GetComponent<Animation>().Play("Damaged");
        if (currHealth <= 0)
        {
            Died();
        }
    }

    public void gainHealth(int hpAmount)
    {
        currHealth += hpAmount;
    }

    private void Died()
    {
        player.GetComponent<Animation>().Stop("Damaged");
        anim.SetBool("isDead", true);
        rb.velocity = new Vector2(0, 0);
        player.GetComponent<PlayerController>().enabled = false;
    }

}
