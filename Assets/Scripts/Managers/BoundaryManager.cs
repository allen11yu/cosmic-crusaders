using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoundaryManager : MonoBehaviour
{
    [Header("Boundary Sphere")]
    public GameObject BoundarySphere;


    [Header("UI Settings")]
    public Image screenOverlay;       // UI Image for red fade overlay
    public TMP_Text countdownText;    // TextMeshPro text for countdown message
    public float countdownDuration = 5f;  // Countdown time before health reaches zero


    private Transform playerTransform;
    private PlayerStats playerStats;

    private float currentTime = 0f;
    private bool outsideBoundary = false;
    private bool gameOverTriggered = false;

    private float boundaryRadius;
    private Vector3 boundaryCenter;

    void Start()
    {
        // Automatically find the PlayerStats component on the player if not assigned
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
        playerObject.GetComponent<PlayerStats>();

        boundaryCenter = BoundarySphere.transform.position;
        boundaryRadius = BoundarySphere.transform.localScale.x * 0.5f;

        // Hide the red overlay at the start
        screenOverlay.gameObject.SetActive(false);
    }

    void Update()
    {
    // If game over has already been triggered, do nothing
        if (gameOverTriggered)
        {
            return;
        }

    // Check if the player is outside boundary
        if (IsPlayerOutsideBoundary())
        {
            if (!outsideBoundary)
            {
                StartLeavingBoundary();  // Start the countdown when player leaves boundary
            }

        // Decrease the countdown timer
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0); // Ensure currentTime doesn't go negative

            UpdateCountdownUI();  // Update the countdown UI and screen fade

        // When the countdown reaches zero, set player health to zero
            if (currentTime == 0)
            {
                DecreasePlayerHealthToZero();
                TriggerGameOver();
            }
        }

        else
        {
            if (outsideBoundary)
            {
            ReturnToBoundary();  // Reset when player returns within the boundary
            }
        }
    }


    // Check if the player is outside the boundary
    bool IsPlayerOutsideBoundary()
    {
        float distanceFromCenter = Vector3.Distance(playerTransform.position, boundaryCenter);
        return distanceFromCenter > boundaryRadius;
    }

    // Called when player leaves the boundary for the first time
    void StartLeavingBoundary()
    {
        outsideBoundary = true;
        currentTime = countdownDuration;  // Reset the countdown
        screenOverlay.color = new Color(1, 0, 0, 0);  // Set the screen overlay to start transparent
        screenOverlay.gameObject.SetActive(true);  // Show the screen overlay
        countdownText.gameObject.SetActive(true);  // Show the countdown text
        BoundarySphere.SetActive(true); //Show sphere boundary
    }

    // Updates the screen fade and countdown timer
    void UpdateCountdownUI()
    {
        // Fade the red overlay from transparent to solid red as time runs out
        screenOverlay.color = new Color(1, 0, 0, Mathf.Lerp(0, 1, 1 - (currentTime / countdownDuration)));

        // Update the countdown text to display the remaining time
        countdownText.text = "Return to the play area! " + Mathf.Ceil(currentTime).ToString();
    }

    // Called when player returns to the play area
    void ReturnToBoundary()
    {
        outsideBoundary = false;
        screenOverlay.gameObject.SetActive(false);  // Hide the screen overlay
        countdownText.gameObject.SetActive(false);  // Hide the countdown text
        BoundarySphere.SetActive(false); //Hide sphere boundary
    }

    // Decrease player's health to zero when they stay outside the boundary
    void DecreasePlayerHealthToZero()
    {
        // Call the TakeDamage method with the current health to reduce health to zero
        playerStats.Die();

        Debug.Log("Player health set to 0 due to boundary exit.");

        // Hide the red overlay and countdown after the player dies
        HideOverlay();

        // Trigger the game over logic
        TriggerGameOver();
    }

    // Hides the screen overlay and countdown text
    void HideOverlay()
    {
        screenOverlay.gameObject.SetActive(false);  // Hide the red screen overlay
        countdownText.gameObject.SetActive(false);  // Hide the countdown text
    }

    // Trigger game over logic
    void TriggerGameOver()
    {
        gameOverTriggered = true;  // Prevent further updates after game over

        GameEndManager.Instance.TriggerGameEnd(false);  

        Debug.Log("Game Over triggered.");
    }
}
