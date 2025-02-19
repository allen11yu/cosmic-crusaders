using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCrackEffect : MonoBehaviour
{
    public Image crackLevel1;  // Light crack image
    public Image crackLevel2;  // Medium crack image
    public Image crackLevel3;  // Heavy crack image
    public PlayerStats playerStats;  // Reference to the PlayerStats script

    public float fadeOutDuration = 1f;  // Duration for the crack to fade out after being hit

    private Coroutine currentFadeCoroutine;

    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();  // Automatically find the PlayerStats script
        }

        // Initialize images to be disabled at start (no cracks visible)
        crackLevel1.gameObject.SetActive(false);
        crackLevel2.gameObject.SetActive(false);
        crackLevel3.gameObject.SetActive(false);

        // Subscribe to health changes
        PlayerStats.HealthChanged += UpdateCrackEffect;
    }

    void UpdateCrackEffect(int currentHealth, int maxHealth)
    {
        // Calculate health percentage
        float healthPercentage = (float)currentHealth / maxHealth;

        // Reset all cracks to be inactive
        crackLevel1.gameObject.SetActive(false);
        crackLevel2.gameObject.SetActive(false);
        crackLevel3.gameObject.SetActive(false);

        // Determine which crack image to show based on health
        if (healthPercentage <= 0.7f && healthPercentage > 0.4f)
        {
            // Show light crack
            ShowCrack(crackLevel1);
        }
        else if (healthPercentage <= 0.4f && healthPercentage > 0.1f)
        {
            // Show medium crack
            ShowCrack(crackLevel2);
        }
        else if (healthPercentage <= 0.1f)
        {
            // Show heavy crack
            ShowCrack(crackLevel3);
        }
    }

    private void ShowCrack(Image crackImage)
    {
        // Stop any current fade-out coroutine to show the new crack image immediately
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        // Enable the selected crack image
        crackImage.gameObject.SetActive(true);

        // Start a fade-out coroutine for this crack image
        currentFadeCoroutine = StartCoroutine(FadeOutCrack(crackImage));
    }

    private IEnumerator FadeOutCrack(Image crackImage)
    {
        Color originalColor = crackImage.color;
        float elapsedTime = 0f;

        // Fade out the crack over time
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            crackImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Hide the crack image after fading out
        crackImage.gameObject.SetActive(false);
        crackImage.color = originalColor;  // Reset to original color for the next time it's shown
    }

    void OnDestroy()
    {
        // Unsubscribe from the health change event to avoid memory leaks
        PlayerStats.HealthChanged -= UpdateCrackEffect;
    }
}
