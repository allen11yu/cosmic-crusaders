using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostBar : MonoBehaviour
{
    public Image fillImage;
    private PlayerStats playerStats;
    private float currentBoost;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        currentBoost = playerStats.maxBoost;
    }

    public void UpdateBoostBar(float boost)
    {
        currentBoost = boost;
        fillImage.fillAmount = currentBoost / playerStats.maxBoost;
    }
}