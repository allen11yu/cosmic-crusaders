using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI mineralText;
    public TextMeshProUGUI boostText;

    void OnEnable()
    {
        PlayerStats.ExperienceChanged += UpdateExperience;
        PlayerStats.LevelChanged += UpdateLevel;
        PlayerStats.HealthChanged += UpdateHealth;
        PlayerStats.MineralChanged += UpdateMineral;
        PlayerStats.BoostChanged += UpdateBoost;
    }

    void OnDisable()
    {
        PlayerStats.ExperienceChanged -= UpdateExperience;
        PlayerStats.LevelChanged -= UpdateLevel;
        PlayerStats.HealthChanged -= UpdateHealth;
        PlayerStats.MineralChanged -= UpdateMineral;
        PlayerStats.BoostChanged -= UpdateBoost;
    }

    void UpdateExperience(int experience)
    {
        experienceText.text = "Experience: " + experience.ToString();
    }

    void UpdateLevel(int level)
    {
        levelText.text = "Level: " + level.ToString();
    }

    void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"Health: {currentHealth}/{maxHealth}";
    }

    void UpdateMineral(int mineral)
    {
        mineralText.text = "Minerals: " + mineral.ToString();
    }

    void UpdateBoost(int currentBoost, int maxBoost)
    {
        boostText.text = $"Boost: {currentBoost}/{maxBoost}";
    }
}
