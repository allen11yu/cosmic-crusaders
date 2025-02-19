using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IShootingStats
{
    [Header("Player Experience")]
    public int currentLevel = 1;
    public int currentExperience = 0;

    [Header("Weapon Upgrade")]
    public bool isMissileUnlock = false;
    public bool isShieldUnlock = false;
    public bool isGravityBombUnlock = false;


    [Header("Player Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    [Header("Player Boost")]
    public int maxBoost = 500;
    public int currentBoost;
    public BoostBar boostBar;

    [Header("Player Combat")]
    public float damage = 10f;
    public float damageOnCollision = 5f;
    public float bulletSpeed = 30f;
    public float bulletLifespan = 1f;
    public float fireRate = 0.5f;

    [Header("Player Mineral Count")]
    public int currentMineral = 0;

    [Header("Player Movement")]
    public float speed = 30f;
    public float idleSpeed = 5f;
    public float boostSpeed = 2f;
    public float forwardAcceleration = 2.5f;
    public float lookRotateSpeed = 60f;
    public float rollSpeed = 90f;
    public float rollAcceleration = 3.5f;

    [Header("Leveling Configuration")]
    public PlayerLevelData levelData;

    private CameraShake cameraShake;

    public float BulletSpeed => bulletSpeed;
    public float FireRate => fireRate;
    public float Damage => damage;
    public float BulletLifespan => bulletLifespan;

    public delegate void OnExperienceChanged(int experience);
    public static event OnExperienceChanged ExperienceChanged;

    public delegate void OnLevelChanged(int level);
    public static event OnLevelChanged LevelChanged;

    public delegate void OnHealthChanged(int health, int maxHealth);
    public static event OnHealthChanged HealthChanged;

    public delegate void OnMineralChanged(int mineral);
    public static event OnMineralChanged MineralChanged;

    public delegate void OnBoostChanged(int boost, int maxBoost);
    public static event OnBoostChanged BoostChanged;

    public GameObject levelUpPanel;


    void Start()
    {
        currentHealth = maxHealth;
        currentBoost = maxBoost;

        cameraShake = Camera.main.GetComponent<CameraShake>();
        if (cameraShake == null)
        {
            Debug.LogError("No CamaraShake script on MainCamera");
        }
        HealthChanged?.Invoke(currentHealth, maxHealth);
        LevelChanged?.Invoke(currentLevel);
        ExperienceChanged?.Invoke(currentExperience);
        MineralChanged?.Invoke(currentMineral);
        BoostChanged?.Invoke(currentBoost, maxBoost);
    }

    public void AddExperience(int amount)
    {
        currentExperience += amount;
        ExperienceChanged?.Invoke(currentExperience);
        CheckLevelUp();
    }

    public void AddMineral(int amount)
    {
        currentMineral += amount;
        MineralChanged?.Invoke(currentMineral);
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        HealthChanged?.Invoke(currentHealth, maxHealth);
        healthBar.UpdateHealthBar(currentHealth);
    }

    public void AddBoost(int amount)
    {
        currentBoost += amount;
        BoostChanged?.Invoke(currentBoost, maxBoost);
        boostBar.UpdateBoostBar(currentBoost);
    }

    private void CheckLevelUp()
    {
        if (currentLevel - 1 >= levelData.experienceRequiredPerLevel.Length)
            return;

        int requiredExp = levelData.experienceRequiredPerLevel[currentLevel - 1];
        if (currentExperience >= requiredExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExperience = 0;
        if (currentLevel >= 5)
        {
            // Win condition
            GameEndManager.Instance.TriggerGameEnd(true);
            return;
        }
        ExperienceChanged?.Invoke(currentExperience);
        LevelChanged?.Invoke(currentLevel);

        //levelUpPanel.SetActive(true);

        Debug.Log("Leveled up to level " + currentLevel);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= (int)Math.Floor(damageAmount);
        HealthChanged?.Invoke(currentHealth, maxHealth);
        healthBar.UpdateHealthBar(currentHealth);

        // Trigger camera shake when taking damage
        if (cameraShake != null)
        {
            cameraShake.TriggerShake();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player has died.");
        GameEndManager.Instance.TriggerGameEnd(false);
    }

    // Methods to Increase Stats
    public void IncreaseHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        HealthChanged?.Invoke(currentHealth, maxHealth);
        healthBar.UpdateHealthBar(currentHealth);
        Debug.Log($"Health increased by {amount}. Max Health: {maxHealth}");
    }

    public void IncreaseDamage(float amount)
    {
        damage += amount;
        Debug.Log($"Damage increased by {amount}. Damage: {damage}");
    }

    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log($"Speed increased by {amount}. Idle speed: {speed}");
    }

    public void IncreaseIdleSpeed(float amount)
    {
        idleSpeed += amount;
        Debug.Log($"Idle speed increased by {amount}. Idle speed: {idleSpeed}");
    }

    public void IncreaseBoostSpeed(float amount)
    {
        boostSpeed += amount;
        Debug.Log($"Boost speed increased by {amount}. Boost speed: {boostSpeed}");
    }

    public void IncreaseTurnSpeed(float amount)
    {
        lookRotateSpeed += amount;
        Debug.Log($"Turn speed increased by {amount}. Turn speed: {lookRotateSpeed}");
    }
}
