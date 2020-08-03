using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currHealth;
    private GameObject player;

    private void Awake()
    {
        //currHealth = 10;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        Debug.Log(currHealth);
        if (currHealth <= 0)
        {
            Debug.Log("Died");
        }
    }

    public void takeDamage(int dmgAmount)
    {
        currHealth -= dmgAmount;
        player.GetComponent<Animation>().Play("Damaged");
    }

    public void gainHealth(int hpAmount)
    {
        currHealth += hpAmount;
    }

}
