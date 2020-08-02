using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currHealth;

    private void Awake()
    {
        //currHealth = 10;
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
    }

    public void gainHealth(int hpAmount)
    {
        currHealth += hpAmount;
    }

}
