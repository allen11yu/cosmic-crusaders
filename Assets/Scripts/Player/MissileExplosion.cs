using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileExplosion : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float explosionDelay = 2f;
    public float explosionRadius = 10f;
    public float explosionDamage = 20f;

    private bool hasExploded = false;
    public AudioClip destroySound;

    void Start()
    {
        // Explode after a few seconds if there is no collision
        if (!hasExploded)
        {
            Invoke("Explode", explosionDelay);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        CancelInvoke("Explode");
        if (!hasExploded)
        {
            Explode();
            Destroy(gameObject);
            Destroy(explosionPrefab, 2f);
        }
    }


    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;
        CancelInvoke("Explode");
        Debug.Log("Explosion");

        PlayDestroySound();
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(explosion, 2f);
        }

        // Objects within the sphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            EnemyStats enemy = collider.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }

            PlayerStats player = collider.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
            }
        }

        Destroy(explosionPrefab, 2f);
        Destroy(gameObject);
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