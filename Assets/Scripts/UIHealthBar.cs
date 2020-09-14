using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image healthImage;
    [SerializeField] private Image healthShrinkImage;
    [SerializeField] private float shrinkTimer;
    [SerializeField] private float shrinkSpeed;

    private float _shrinkTimer;

    public void SetHealth(float currEnemyHealth, float maxEnemyHealth)
    {
        healthBar.SetActive(currEnemyHealth < maxEnemyHealth);
        _shrinkTimer = shrinkTimer;
        healthImage.fillAmount = currEnemyHealth/maxEnemyHealth;
    }

    private void Update()
    {
        _shrinkTimer -= Time.deltaTime;
        if (_shrinkTimer < 0)
        {
            if (healthImage.fillAmount < healthShrinkImage.fillAmount)
            {
                healthShrinkImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }

    public void enemyDied(bool isDead)
    {
        healthBar.SetActive(!isDead);
    }
}
