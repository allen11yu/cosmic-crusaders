using System.Collections;
using UnityEngine;
using TMPro;

public class WaveUI : MonoBehaviour
{
    public TextMeshProUGUI waveText;
    public float fadeDuration = 3f;

    public void ShowWaveText(int waveNumber)
    {
        waveText.text = "WAVE " + waveNumber + ": DESTORY All ENEMIES.";
        StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        waveText.alpha = 1f;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            waveText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        waveText.alpha = 0f;
    }
}