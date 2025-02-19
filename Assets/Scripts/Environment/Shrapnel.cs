using UnityEngine;

public class Shrapnel : MonoBehaviour
{
    public float damage = 2f;
    public ParticleSystem effect;
    public AudioClip metalClank; 

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            if (effect != null)
            {
                ContactPoint contact = collision.contacts[0]; 
                ParticleSystem particle = Instantiate(effect, contact.point, Quaternion.identity); 
                particle.Play(); 

                if (metalClank != null)
                {
                    AudioSource audioSource = GetComponent<AudioSource>();  
                    if (audioSource != null)
                    {
                        audioSource.PlayOneShot(metalClank);  
                    }
                }
            }

            Destroy(gameObject, 1f);
        }
    }
}




