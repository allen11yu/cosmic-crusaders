using System.Collections;
using UnityEngine;
using System;

public class EnemyStats : MonoBehaviour, IShootingStats
{
    [Header("Enemy Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    public int experienceReward = 5;
    public int mineralReward = 5;
    public EnemyHealthBar healthBar;


    [Header("Enemy Combat")]
    public float damage = 10f;
    public float damageOnCollision = 5f;
    public float bulletSpeed = 30f;
    public float bulletLifespan = 1f;
    public float fireRate = 0.5f;
    public float attackRange = 10f;
    public double accuracy = 0.95;


    [Header("Enemy Movement")]
    public float speed = 20f;
    public float idleSpeed = 5f;
    public float boostSpeed = 3f;
    public float forwardAcceleration = 2.5f;
    public float lookRotateSpeed = 35f;
    public float rollSpeed = 5f;
    public float rollAcceleration = 3.5f;


    [Header("Damage Feedback")]
    public Color hitColor = Color.red;
    public float hitEffectDuration = 1f;


    public float BulletSpeed => bulletSpeed;
    public float FireRate => fireRate;
    public float Damage => damage;
    public float BulletLifespan => bulletLifespan;

    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged HealthChanged;
    public delegate void OnDamageTaken();
    public event OnDamageTaken DamageTaken;

    private Color originalColor;   // Store the original color
    private Renderer enemyRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);

        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
        }

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
        else
        {
            Debug.LogError("Renderer component not found on enemy or its children.");
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= (int)Math.Floor(damageAmount);
        HealthChanged?.Invoke(currentHealth, maxHealth);
        healthBar.UpdateHealthBar(currentHealth);

        DamageTaken?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(ShowHitEffect());
        }
    }

    private IEnumerator ShowHitEffect()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;

            yield return new WaitForSeconds(hitEffectDuration);

            enemyRenderer.material.color = originalColor;
        }
    }

    public void Die()
    {
        Debug.Log("Enemy has died.");
        if (gameObject.CompareTag("FinalBoss"))
        {
            GameEndManager.Instance.TriggerGameEnd(true);
        } else
        {
            GiveExperienceMineralToPlayer();
        }
        Destroy(gameObject);
    }

    private void GiveExperienceMineralToPlayer()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.AddExperience(experienceReward);
            Debug.Log($"Player gained {experienceReward} experience for killing the enemy.");

            playerStats.AddMineral(mineralReward);
            Debug.Log($"Player gain {mineralReward} mineral for killing the enemy.");
        }
    }
}
