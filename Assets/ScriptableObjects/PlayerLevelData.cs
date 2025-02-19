using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLevelData", menuName = "Game Data/Player Level Data")]
public class PlayerLevelData : ScriptableObject
{
    public int[] experienceRequiredPerLevel;
    public int baseExperience = 5; // Starting experience for level 1
    public int totalLevels = 5; // Total number of levels

    private void OnValidate()
    {
        if (experienceRequiredPerLevel == null || experienceRequiredPerLevel.Length != totalLevels)
        {
            experienceRequiredPerLevel = new int[totalLevels];
        }

        experienceRequiredPerLevel[0] = baseExperience;
        for (int i = 1; i < totalLevels; i++)
        {
            experienceRequiredPerLevel[i] = Mathf.CeilToInt(experienceRequiredPerLevel[i - 1] * 1.1f);
        }
    }
}

