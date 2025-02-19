using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HomingMissile : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float explosionDelay = 5f;
    public float explosionRadius = 5f;
    public float explosionDamage = 4f;

    public float speed = 1000f;
    public float turnSpeed = 100f;

    private Transform target;
    private bool hasExploded = false;
    public AudioClip destroySound;

    void Start()
    {
        // Find the closest enemy
        FindClosestTarget();

        // and then explode after a few seconds after instantiation
        Invoke("Explode", explosionDelay);
    }

    void Update()
    {
        if (target != null)
        {
            if (HasLineOfSight(target))
            {
                Vector3 direction = target.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        else
        {
            target = transform;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("FinalBoss") ||
            collision.gameObject.CompareTag("Asteroid") ||
            collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") ||
            other.gameObject.CompareTag("FinalBoss") ||
            other.gameObject.CompareTag("Asteroid") ||
            other.gameObject.CompareTag("Player"))
        {
            Explode();
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
            PlayerStats player = collider.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
            }

            EnemyStats enemy = collider.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject); 
    }

    void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] boss = GameObject.FindGameObjectsWithTag("FinalBoss");
        GameObject[] targets = enemies.Concat(boss).ToArray();

        float closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in targets)
        {
            Transform enemyTransform = enemy.transform;
            float distance = Vector3.Distance(transform.position, enemyTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;

                target = enemyTransform;
            }
        }
    }

    bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit))
        {
            if (hit.transform == target)
            {
                return true;
            }
        }
        return false;
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
