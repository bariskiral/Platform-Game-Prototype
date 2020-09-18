using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthImage;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Image healthShrinkImage;
    [SerializeField] private float shrinkTimer;
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private float shieldTimer;

    private float _shrinkTimer;
    private float _shieldTimer;

    public void SetHealth(float currEnemyHealth, float maxEnemyHealth)
    {
        gameObject.SetActive(currEnemyHealth < maxEnemyHealth);
        _shrinkTimer = shrinkTimer;
        _shieldTimer = shieldTimer;
        healthImage.fillAmount = currEnemyHealth / maxEnemyHealth;
    }

    public void SetShield(float currEnemyShield, float maxEnemyShield)
    {
        gameObject.SetActive(currEnemyShield < maxEnemyShield);
        _shieldTimer = shieldTimer;
        shieldImage.fillAmount = currEnemyShield / maxEnemyShield;
    }

    private void Update()
    {
        _shrinkTimer -= Time.deltaTime;
        _shieldTimer -= Time.deltaTime;

        if (_shrinkTimer < 0)
        {
            if (healthImage.fillAmount < healthShrinkImage.fillAmount)
            {
                healthShrinkImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }

        if (_shieldTimer < 0)
        {
            shieldImage.fillAmount += shrinkSpeed * Time.deltaTime;
            //enemy._enemyShield = enemy.enemyShield;
            GetComponentInParent<EnemyController>()._enemyShield = GetComponentInParent<EnemyController>().enemyShield;
        }
    }

    public void enemyDied(bool isDead)
    {
        gameObject.SetActive(!isDead);
    }
}
