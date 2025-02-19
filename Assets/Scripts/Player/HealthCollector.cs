using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollector : MonoBehaviour
{
    [Header("Health Collection")]
    public PlayerStats playerStats; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUpHealth"))
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                float newHealth = playerStats.currentHealth + health.healthValue;

                if (newHealth > playerStats.maxHealth)
                {
                    playerStats.currentHealth = playerStats.maxHealth;
                }
                else
                {
                    playerStats.AddHealth(health.healthValue);
                }
                Destroy(other.gameObject);
            }
        }
    }
}


