using UnityEngine;

public class BoostTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private float fadeDuration = 2f;
    private bool isFading;

    public GameObject player; 
    public ParticleSystem leftBig; 
    public ParticleSystem rightBig; 

    private PlayerStats playerStats; 

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;

        leftBig.Stop();
        rightBig.Stop();

        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        bool isBoostKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool isBoostActive = playerStats != null && playerStats.currentBoost > 0;

        if (isBoostKeyPressed && isBoostActive)
        {
            trailRenderer.enabled = true;
            trailRenderer.startWidth = 0.05f;
            isFading = false;

            if (!leftBig.isPlaying) leftBig.Play();
            if (!rightBig.isPlaying) rightBig.Play();
        }
        else if (trailRenderer.enabled && !isFading)
        {
            StartFading();
        }

        if (!isBoostActive)
        {
            leftBig.Stop();
            rightBig.Stop();
        }

        if (isFading)
        {
            trailRenderer.startWidth -= (Time.deltaTime / fadeDuration);

            if (trailRenderer.startWidth <= 0)
            {
                trailRenderer.startWidth = 0;
                trailRenderer.enabled = false;
                isFading = false;

                leftBig.Stop();
                rightBig.Stop();
            }
        }
    }

    private void StartFading()
    {
        isFading = true;
    }
}