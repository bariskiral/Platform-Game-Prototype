using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEnemyStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthPoint;
    [SerializeField] private TextMeshProUGUI attackPoint;
    [SerializeField] private EnemyController enemy;

    private void Update()
    {
        healthPoint.text = enemy._enemyHealth.ToString();
        attackPoint.text = enemy.enemyDamage.ToString();

        if (enemy._enemyHealth <= 0)
        {
            healthPoint.enabled = false;
            attackPoint.enabled = false;
        }
    }

}
