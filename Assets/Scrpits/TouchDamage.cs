using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{

    private GameObject player;
    private PlayerHealth playerHealth;

    [SerializeField] private float currTime;
    [SerializeField] private float nextDmg;
    [SerializeField] private int damageValue;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player && currTime<=0)
        {
            playerHealth.takeDamage(damageValue);
            currTime = nextDmg;
        }
        else
        {
            currTime -= Time.deltaTime;
        }
    }
}
