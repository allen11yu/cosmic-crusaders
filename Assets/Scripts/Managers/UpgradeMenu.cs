using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class UpgradeMenu : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public static bool canPause = true;
    public PlayerStats playerStats;

    [Header("Left Side: Level-Based Upgrades")]
    public TextMeshProUGUI leftTitle;
    public Button[] leftUpgradeButtons;
    private string[] leftStatUpgrades = { "25%", "25%", "25%", "25%", "25%" };

    [Header("Right Side: Mineral-Based Upgrades")]
    public TextMeshProUGUI rightTitle;
    public Button[] rightUpgradeButtons;
    private int[] rightUpgradeCosts = { 5, 10, 15 };
    private string[] rightWeaponUpgrades = { "5 minerals", "10 minerals", "15 minerals" };

    private int prevLevel;
    private bool isMenuOpen = false;

    void Start()
    {
        prevLevel = playerStats.currentLevel;
        UpdateLeftTitle();
        UpdateRightTitle();
        InitLeftButtons();
        InitRightButtons();
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) Debug.LogError("No canvas group");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.M) && canPause)
        {
            isMenuOpen = !isMenuOpen;
            canvasGroup.interactable = isMenuOpen;
            canvasGroup.blocksRaycasts = isMenuOpen;
            Cursor.visible = isMenuOpen;
            canvasGroup.alpha = isMenuOpen ? 1f : 0f;
            Time.timeScale = isMenuOpen ? 0f : 1f;
        }

        if (playerStats.currentLevel > prevLevel)
        {
            OnPlayerLevelUp();
        }
        UpdateRightTitle();
    }

    private void UpdateLeftTitle()
    {
        leftTitle.text = $"Upgrades for level {playerStats.currentLevel}";
    }

    private void UpdateRightTitle()
    {
        rightTitle.text = $"Weapon Upgrades: {playerStats.currentMineral} minerals available";
    }

    private void InitLeftButtons()
    {
        EnableLeftButtons();
        HashSet<int> currUpgrades = new HashSet<int>();

        for (int i = 0; i < leftUpgradeButtons.Length; i++)
        {
            int randomUpgradeIndex = Random.Range(0, leftStatUpgrades.Length);
            while (currUpgrades.Contains(randomUpgradeIndex))
            {
                randomUpgradeIndex = Random.Range(0, leftStatUpgrades.Length);
            }
            currUpgrades.Add(randomUpgradeIndex);
            leftUpgradeButtons[i].onClick.AddListener(() => OnLeftButtonClick(randomUpgradeIndex));

            TextMeshProUGUI buttonText = leftUpgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = leftStatUpgrades[randomUpgradeIndex];
        }

    }

    private void InitRightButtons()
    {
        for (int i = 0; i < rightUpgradeButtons.Length; i++)
        {
            int currIndex = i;
            rightUpgradeButtons[i].onClick.AddListener(() => OnRightButtonClick(currIndex));

            TextMeshProUGUI buttonText = rightUpgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = rightWeaponUpgrades[i];
        }
    }

    private void OnLeftButtonClick(int index)
    {
        Debug.Log($"Button {index + 1} sleected for level {playerStats.currentLevel}");

        // disable all left buttons until new upgrades are available (i.e. next level)
        foreach (var button in leftUpgradeButtons)
        {
            button.interactable = false;
        }

        // update player stats based on which index its clicked.
        if (index == 0)
        {
            playerStats.IncreaseHealth(Mathf.CeilToInt(playerStats.currentHealth * 0.25f));
        }
        else if (index == 1)
        {
            playerStats.IncreaseDamage(Mathf.CeilToInt(playerStats.damage * 0.25f));
        }
        else if (index == 2)
        {
            playerStats.IncreaseSpeed(Mathf.CeilToInt(playerStats.speed * 0.25f));
        }
        else if (index == 3)
        {
            playerStats.IncreaseIdleSpeed(Mathf.CeilToInt(playerStats.idleSpeed * 0.25f));
        }
        else if (index == 4)
        {
            playerStats.IncreaseBoostSpeed(Mathf.CeilToInt(playerStats.boostSpeed * 0.25f));
        }
        Cursor.visible = true;
    }

    private void OnRightButtonClick(int index)
    {
        if (playerStats.currentMineral >= rightUpgradeCosts[index])
        {
            //playerStats.currentMineral -= rightUpgradeCosts[index];
            playerStats.AddMineral(-1 * rightUpgradeCosts[index]);
            Debug.Log($"Right Upgrade {index + 1} purchased!");
            rightUpgradeButtons[index].interactable = false;

            // enable the key press.
            if (rightUpgradeCosts[index] == 5)
            {
                playerStats.isMissileUnlock = true;
            }
            else if (rightUpgradeCosts[index] == 10)
            {
                playerStats.isShieldUnlock = true;
            }
            else if (rightUpgradeCosts[index] == 15)
            {
                playerStats.isGravityBombUnlock = true;
            }
        }
        else
        {
            Debug.Log("Not enough minerals");
            rightTitle.text = "Not enough minerals";
            // need working;
            //ShowNotEnoughMineralsMessage();
        }
        UpdateRightTitle();
        Cursor.visible = true;
    }

    private void OnPlayerLevelUp()
    {
        prevLevel = playerStats.currentLevel;
        UpdateLeftTitle();
        InitLeftButtons();
    }

    private void EnableLeftButtons()
    {
        foreach (var button in leftUpgradeButtons)
        {
            button.interactable = true;
        }
    }

    private void ShowNotEnoughMineralsMessage()
    {
        Cursor.visible = true;
        string ogRightTitle = rightTitle.text;
        rightTitle.text = "Not enough minerals";
        //yield return new WaitForSecondsRealtime(2);
        //rightTitle.text = ogRightTitle;
    }
}