using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelUpPanel;

    private PlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found in the scene.");
            return;
        }
        
        Time.timeScale = 0f;
        Cursor.visible = true;
        PauseMenuToggle.canPause = false;
    }

    public void OnHealthButtonClicked()
    {
        playerStats.IncreaseHealth(Mathf.CeilToInt(playerStats.currentHealth * 0.1f));
        Debug.Log("Health increased by 10%.");
        CloseLevelUpPanel();
    }

    public void OnDamageButtonClicked()
    {
        playerStats.IncreaseDamage(Mathf.CeilToInt(playerStats.damage * 0.05f));
        Debug.Log("Damage increased by 5%.");
        CloseLevelUpPanel();
    }

    public void OnSpeedButtonClicked()
    {
        playerStats.IncreaseSpeed(Mathf.CeilToInt(playerStats.speed * 0.05f));
        Debug.Log("Speed increased by 5%.");
        CloseLevelUpPanel();
    }

    public void OnIdleSpeedButtonClicked()
    {
        playerStats.IncreaseIdleSpeed(Mathf.CeilToInt(playerStats.idleSpeed * 0.05f));
        Debug.Log("Idle speed increased by 5%.");
        CloseLevelUpPanel();
    }

    public void OnBoostSpeedButtonClicked()
    {
        playerStats.IncreaseBoostSpeed(Mathf.CeilToInt(playerStats.boostSpeed * 0.05f));
        Debug.Log("Boost speed increased by 5%.");
        CloseLevelUpPanel();
    }

    public void CloseLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
        PauseMenuToggle.canPause = true;
        Cursor.visible = false;
        Time.timeScale = 1f;
        Debug.Log("Level Up Popup Closed.");
    }
}
