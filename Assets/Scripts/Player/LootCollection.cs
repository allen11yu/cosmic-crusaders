using UnityEngine;
using TMPro;

public class LootCollection : MonoBehaviour
{
    [Header("Loot Collection")]
    public PlayerStats playerStats;
    public AudioClip pickUpSound;
    private AudioSource audioSource;

    public TextMeshProUGUI itemsInfoText; // Reference to the shared info card TextMeshPro
    public float infoCardDuration = 3f; // How long the info card stays visible

    public float attractionForce = 10f;  // Strength of the gravitational pull
    public float pickupRange = 5f;  // Maximum distance at which the mineral starts moving towards the player

    private Transform player;  // Reference to the player's transform
    private Rigidbody rb;  // Reference to the mineral's Rigidbody

    private void Start()
    {
        // Ensure the mineral has a Rigidbody component
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing from the mineral object!");
            return;
        }

        // Find the player's transform (assuming the player has the tag "Player")
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the tag 'Player'");
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        if (itemsInfoText != null)
        {
            itemsInfoText.gameObject.SetActive(false); // Ensure the info card is hidden initially
        }
    }

    private void Update()
    {
        if (player != null && rb != null)
        {
            // Calculate distance between the mineral and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Only apply attraction if within range
            if (distanceToPlayer <= pickupRange)
            {
                // Calculate direction to player
                Vector3 directionToPlayer = (player.position - transform.position).normalized;

                // Apply a force towards the player
                rb.AddForce(directionToPlayer * attractionForce * Time.deltaTime, ForceMode.VelocityChange);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for collisions with objects tagged as "PickUp"
        if (other.CompareTag("PickUpMineral"))
        {
            Mineral mineral = other.GetComponent<Mineral>();
            if (mineral != null)
            {
                // Add mineral value to player stats
                playerStats.AddMineral(mineral.mineralValue);

                // Play the pickup sound
                if (pickUpSound != null)
                {
                    audioSource.PlayOneShot(pickUpSound);
                }
                ShowInfoCard($"You gathered {mineral.mineralValue} Mineral Resources! Use them to upgrade/purchase weapons and improve your ship.");

                // Destroy the mineral object after pickup
                Destroy(other.gameObject);
            }
            Debug.Log("Picked up mineral");
        }
        else if (other.CompareTag("PickUpHealth"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                playerStats.AddHealth(health.healthValue);

                // Play the pickup sound
                if (pickUpSound != null)
                {
                    audioSource.PlayOneShot(pickUpSound);
                }
                ShowInfoCard($"You gained {health.healthValue} Health Points!");
                // Destroy the mineral object after pickup
                Destroy(other.gameObject);
            }
            Debug.Log("Picked up health");
        }
        else if (other.CompareTag("PickUpBoost"))
        {
            Boost boost = other.GetComponent<Boost>();
            if (boost != null)
            {
                playerStats.AddBoost(boost.boostValue);

                // Play the pickup sound
                if (pickUpSound != null)
                {
                    audioSource.PlayOneShot(pickUpSound);
                }
                ShowInfoCard($"You acquired a Boost of {boost.boostValue}!");

                // Destroy the mineral object after pickup
                Destroy(other.gameObject);
            }
        }
    }

    private void ShowInfoCard(string message)
    {
        if (itemsInfoText != null)
        {
            itemsInfoText.text = message;
            itemsInfoText.gameObject.SetActive(true);
            CancelInvoke("HideInfoCard"); // Cancel any previous hiding invocations
            Invoke("HideInfoCard", infoCardDuration);
        }
    }

    private void HideInfoCard()
    {
        if (itemsInfoText != null)
        {
            itemsInfoText.gameObject.SetActive(false);
        }
    }
}

