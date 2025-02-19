using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostCollector : MonoBehaviour
{
    [Header("Boost Collection")]
    public PlayerStats playerStats; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            Boost boost = other.GetComponent<Boost>();

            if (boost != null)
            {
                playerStats.AddBoost(boost.boostValue);
                Destroy(other.gameObject);
            }
        }
    }
}