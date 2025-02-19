using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillImage;
    private PlayerStats playerStats;
    private float currentHealth;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        currentHealth = playerStats.maxHealth;
    }

    public void UpdateHealthBar(float health)
    {
        currentHealth = health;
        fillImage.fillAmount = currentHealth / playerStats.maxHealth;
    }
}