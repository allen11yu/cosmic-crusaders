using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollection : MonoBehaviour
{
    //[Header("Collection Components")]
    public PlayerStats playerStats;
    public AudioClip pickUpSound;
    private AudioSource audioSource;

    public TextMeshProUGUI itemsInfoText; // Reference to the shared info card TextMeshPro
    public float infoCardDuration = 3f; // How long the info card stays visible
    private List<GameObject> activeInfoCards = new List<GameObject>(); // Track active info cards

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (itemsInfoText != null)
        {
            itemsInfoText.gameObject.SetActive(false); // Ensure the info card is hidden initially
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                playerStats.AddHealth(health.healthValue);
                ShowInfoCard($"You gained {health.healthValue} Health Points!");
                Destroy(other.gameObject);
                return;
            }

            Boost boost = other.GetComponent<Boost>();
            if (boost != null)
            {
                playerStats.AddBoost(boost.boostValue);
                ShowInfoCard($"You acquired a Boost of {boost.boostValue}!");
                Destroy(other.gameObject);
                return;
            }

            Mineral mineral = other.GetComponent<Mineral>();
            if (mineral != null)
            {
                playerStats.AddMineral(mineral.mineralValue);
                if (pickUpSound != null)
                {
                    audioSource.PlayOneShot(pickUpSound);
                }
                ShowInfoCard($"You gathered {mineral.mineralValue} Mineral Resources! Use them to upgrade/purchase weapons and improve your ship.");
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
