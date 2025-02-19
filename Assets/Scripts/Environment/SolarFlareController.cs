using System.Collections;
using UnityEngine;

public class SolarFlareController : MonoBehaviour
{
    private ParticleSystem solarFlareParticles; // Reference to the Particle System for the flare
    private Light flareLight; // Reference to the Light component

    // Flare settings
    public float minTimeBetweenFlares = 120f; // Minimum time between flares (e.g., 2 minutes)
    public float maxTimeBetweenFlares = 180f; // Maximum time between flares (e.g., 3 minutes)
    public float flareBlastDuration = 3f;    // Duration of the flare blast
    public float fadeOutDuration = 5f;       // Duration for fading out the flare
    public float lingeringGreenDuration = 10f; // Duration to keep the green lingering
    public Color flareStartColor = Color.yellow; // Initial color of the flare
    public Color flareEndColor = Color.green;    // Color to linger at the end

    void Start()
    {
        // Get the Particle System component
        solarFlareParticles = GetComponent<ParticleSystem>();
        if (solarFlareParticles == null)
        {
            Debug.LogError("No Particle System found on SolarFlare GameObject!");
            return;
        }

        // Get the Light component
        flareLight = GetComponent<Light>();
        if (flareLight != null)
        {
            flareLight.intensity = 0f; // Start with no light
        }

        // Start the solar flare coroutine
        StartCoroutine(TriggerSolarFlares());
    }

    private IEnumerator TriggerSolarFlares()
    {
        while (true)
        {
            // Wait for a random interval between 2-3 minutes
            float waitTime = Random.Range(minTimeBetweenFlares, maxTimeBetweenFlares);
            Debug.Log($"Waiting for {waitTime} seconds before triggering the next solar flare.");
            yield return new WaitForSeconds(waitTime);

            // Trigger the solar flare
            Debug.Log("Solar flare triggered!");
            solarFlareParticles.Play();

            // Gradually increase light intensity
            StartCoroutine(IncreaseLightIntensity());
        }
    }

    private IEnumerator IncreaseLightIntensity()
    {
        float elapsedTime = 0f;

        // Gradually increase light intensity to simulate the flare blast
        while (elapsedTime < flareBlastDuration)
        {
            elapsedTime += Time.deltaTime;
            if (flareLight != null)
            {
                flareLight.intensity = Mathf.Lerp(0, 1, elapsedTime / flareBlastDuration); // Scale intensity
            }
            yield return null;
        }

        // Ensure maximum intensity is reached
        if (flareLight != null)
        {
            flareLight.intensity = 1f;
        }

        // Transition to green color and fade out slowly
        StartCoroutine(FadeOutAndLingerGreen());
    }

    private IEnumerator FadeOutAndLingerGreen()
    {
        float elapsedTime = 0f;

        // Get Particle System's Main module to modify its color
        ParticleSystem.MainModule main = solarFlareParticles.main;

        // Gradually transition to green while fading out the light
        Color initialColor = main.startColor.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            // Fade out light intensity
            if (flareLight != null)
            {
                flareLight.intensity = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            }

            // Gradually change particle color to green
            Color newColor = Color.Lerp(initialColor, flareEndColor, elapsedTime / fadeOutDuration);
            main.startColor = newColor;

            yield return null;
        }

        // Keep the green lingering for a specified duration
        float lingeringTime = 0f;
        while (lingeringTime < lingeringGreenDuration)
        {
            lingeringTime += Time.deltaTime;

            // Maintain the green color
            main.startColor = flareEndColor;

            yield return null;
        }

        // Ensure light is completely off and particles are green at the end
        if (flareLight != null)
        {
            flareLight.intensity = 0f;
        }
        main.startColor = flareEndColor;

        Debug.Log("Solar flare green lingering effect completed.");
    }
}
