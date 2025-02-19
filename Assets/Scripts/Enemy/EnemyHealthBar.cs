using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;
    private EnemyStats enemyStats;
    private float currentHealth;

    void Start()
    {
        enemyStats = GetComponentInParent<EnemyStats>();
        currentHealth = enemyStats.maxHealth;
    }

    public void UpdateHealthBar(float health)
    {
        currentHealth = health;
        fillImage.fillAmount = currentHealth / enemyStats.maxHealth;
    }
}