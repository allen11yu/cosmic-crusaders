using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    private EnemyStats enemyStats; 
    public GameObject[] spaceGarbage; 
    public GameObject explosionPrefab; 
    public GameObject collectable;

    public AudioClip destroySound;

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
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(explosion, 5f);
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



