using UnityEngine;

public class ExperienceCollector : MonoBehaviour
{
    [Header("Experience Collection")]
    public PlayerStats playerStats; // Assign via Inspector

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PickUp"))
        {
            ExperienceObject experience = other.GetComponent<ExperienceObject>();

            if (experience != null)
            {
                playerStats.AddExperience(experience.experienceValue);
                Destroy(other.gameObject);
            }
        }
    }
}
