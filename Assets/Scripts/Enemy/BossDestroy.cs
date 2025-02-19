using UnityEngine;
using System.Collections; 

public class BossDestroy : MonoBehaviour
{
    private EnemyStats enemyStats;
    public GameObject[] spaceGarbage;
    public GameObject[] explosionPrefabs; 
    public GameObject collectable;

    public AudioClip destroySound;

    private bool hasFirstExplosionPlayed = false;
    private bool hasSecondExplosionPlayed = false;

    void Start()
    {
        if (enemyStats == null)
        {
            enemyStats = GetComponent<EnemyStats>();
        }
    }

    private void DestroyEnemy()
    {
        PlayDestroySound();
        StartCoroutine(PlayExplosionsAtHealthThresholds());  
    }

    private IEnumerator PlayExplosionsAtHealthThresholds()
    {
        float healthPercentage = (float)enemyStats.currentHealth / (float)enemyStats.maxHealth;
        Debug.Log("Current Health Percentage: " + healthPercentage * 100 + "%");

        if (healthPercentage > 0.33f && healthPercentage <= 0.66f && !hasFirstExplosionPlayed)
        {
            GameObject explosion = Instantiate(explosionPrefabs[0], transform.position, Quaternion.identity);
            Debug.Log("Explosion 1 triggered at 66% health"); 
            PlayParticleSystem(explosion);  
            hasFirstExplosionPlayed = true; 
            yield return new WaitForSeconds(3f); 
        }


        if (healthPercentage > 0f && healthPercentage <= 0.33f && !hasSecondExplosionPlayed)
        {
            GameObject explosion = Instantiate(explosionPrefabs[1], transform.position, Quaternion.identity);
            Debug.Log("Explosion 2 triggered at 32% health"); 
            PlayParticleSystem(explosion);  
            hasSecondExplosionPlayed = true; 
            yield return new WaitForSeconds(3f); 
        }

        if (healthPercentage <= 0f)
        {
            GameObject explosion = Instantiate(explosionPrefabs[2], transform.position, Quaternion.identity);
            Debug.Log("Explosion 3 triggered at 0% health"); 
            PlayParticleSystem(explosion);  
            yield return new WaitForSeconds(3f); 
        }

        if (collectable != null)
        {
            Instantiate(collectable, transform.position, Quaternion.identity);
        }

        for (int i = 0; i < 5; i++)
        {
            foreach (GameObject prefab in spaceGarbage)
            {
                GameObject droppedItem = Instantiate(prefab, transform.position, Quaternion.identity);
                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    float fragmentSpeed = Random.Range(1f, 3f);
                    rb.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * fragmentSpeed;
                    rb.AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.Impulse);
                }
            }
        }


        Destroy(gameObject);

        yield break; 
    }

    private void PlayParticleSystem(GameObject explosion)
    {
        ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Debug.Log("Particle System found, playing it.");
            ps.Play();  
        }
        else
        {
            Debug.LogError("No ParticleSystem component found on explosion prefab!"); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            if (enemyStats.currentHealth <= 0)
            {
                DestroyEnemy();
            }
        }
    }

    private void PlayDestroySound()
    {
        if (destroySound != null)
        {
            GameObject audioObject = new GameObject("DestroySound");
            AudioSource tempAudioSource = audioObject.AddComponent<AudioSource>();
            tempAudioSource.clip = destroySound;
            tempAudioSource.Play();

            Destroy(audioObject, destroySound.length);
        }
    }
}







