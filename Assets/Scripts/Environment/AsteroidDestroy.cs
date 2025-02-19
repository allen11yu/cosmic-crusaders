using UnityEngine;

public class AsteroidDestroy : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject[] smallerAsteroidPrefabs;
    public GameObject prefab;
    public int numberOfFragments = 20;
    public float explosionDuration = 2f;

    public AudioClip explosionSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DestroyAsteroid();
        }
    }

    public void DestroyAsteroid()
    {
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(explosion, explosionDuration);
        }
        PlayExplosionSound();

        int fragmentsToCreate = numberOfFragments;

        for (int i = 0; i < fragmentsToCreate; i++)
        {
            if (smallerAsteroidPrefabs.Length > 0)
            {
                GameObject smallerAsteroid = smallerAsteroidPrefabs[Random.Range(0, smallerAsteroidPrefabs.Length)];
                GameObject fragment = Instantiate(smallerAsteroid, transform.position, Quaternion.Euler(0, Random.Range(0, 360), 0));

                Rigidbody rb = fragment.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float fragmentSpeed = Random.Range(1f, 3f);
                    rb.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * fragmentSpeed;
                    rb.AddTorque(new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode.Impulse);
                }

                Destroy(gameObject, 0.1f);
            }
        }

        //int mineralDrop = Random.Range(1, 4);
        //for (int i = 0; i < mineralDrop; i++)
        //{
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        //}
    }

    private void PlayExplosionSound()
    {
        if (explosionSound != null)
        {
            GameObject audioObject = new GameObject("ExplosionSound");
            AudioSource tempAudioSource = audioObject.AddComponent<AudioSource>();
            tempAudioSource.clip = explosionSound;
            tempAudioSource.Play();

            Destroy(audioObject, explosionSound.length);
        }
    }

}
