using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GravityBomb : MonoBehaviour
{
    public GameObject gravitybombPrefab;
    public float dropOffset = 2f;
    public float dropForce = 0.5f;
    public float audioDelay = 3.5f; // 3.5 is needed because the mp3 file is a little delayed
    public float detectionRadius = 10f;
    public float gravityDuration = 2f;
    public float delayBeforeGravity = 5f; // this 5 goes with the 3.5 audio delay
    public float replenishCooldown = 30f;
    public PlayerStats playerStats;

    private GameObject currentDroppedObject;
    private AudioSource audioSource;
    private bool isObjectDropped = false;
    private bool isOnCooldown = false;
    private bool damageDealt = false;
    private List<Shield> shieldsHit = new List<Shield>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerStats.isGravityBombUnlock && Input.GetKeyDown(KeyCode.Space) && !isOnCooldown)
        {
            if (!isObjectDropped)
            {
                DropObject();
                StartCoroutine(PlayAudioAfterDelay(audioDelay));
                StartCoroutine(ReplenishCooldown());
            }
        }

        if (isObjectDropped && !damageDealt)
        {
            StartCoroutine(ApplyGravityWithDelay());
        }
    }

    private void DropObject()
    {
        if (gravitybombPrefab != null)
        {
            shieldsHit.Clear();
            damageDealt = false;

            Vector3 dropPosition = transform.position - transform.up * dropOffset;
            currentDroppedObject = Instantiate(gravitybombPrefab, dropPosition, Quaternion.identity);

            Rigidbody rb = currentDroppedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.down * dropForce, ForceMode.Impulse);
            }

            isObjectDropped = true;
        }
    }

    private IEnumerator ApplyGravityWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeGravity);

        ApplyGravityToEnemiesInRange();

        damageDealt = true;
    }

    private HashSet<Collider> enemiesAlreadyDamaged = new HashSet<Collider>();

    private void ApplyGravityToEnemiesInRange()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider enemy in enemiesInRange)
        {

            if (enemiesAlreadyDamaged.Contains(enemy))
            {
                continue;
            }

            if (enemy.CompareTag("Enemy"))
            {
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                BasicEnemyAI enemyAI = enemy.GetComponent<BasicEnemyAI>();
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();

                if (rb != null && !rb.useGravity)
                {
                    StartCoroutine(EnableGravityForDuration(enemy, enemyAI));
                }

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(20f);
                    enemiesAlreadyDamaged.Add(enemy);
                }
            }

            Shield shield = enemy.GetComponentInChildren<Shield>();
            if (shield != null && !shieldsHit.Contains(shield))
            {
                shield.shieldHealth -= 3;
                shieldsHit.Add(shield);
                Debug.Log("Shield damaged by GravityBomb. Remaining health: " + shield.shieldHealth);

                if (shield.shieldHealth <= 0)
                {
                    Destroy(shield.gameObject);
                    Debug.Log("Shield destroyed.");
                }
            }

            /*if (enemy.CompareTag("Asteroid"))
            {
                AsteroidDestroy asteroidDestroy = enemy.GetComponent<AsteroidDestroy>();
                if (asteroidDestroy != null)
                {

                    AudioSource asteroidAudio = asteroidDestroy.GetComponent<AudioSource>();
                    if (asteroidAudio != null)
                    {
                        asteroidAudio.Stop();  
                        asteroidAudio.enabled = false; 
                    }

                    asteroidDestroy.DestroyAsteroid();  // Destroy the asteroid
                    Debug.Log("Asteroid destroyed, no audio played.");
                }
            }*/


        }
    }

    private IEnumerator EnableGravityForDuration(Collider enemyCollider, BasicEnemyAI enemyAI)
    {
        Rigidbody rb = enemyCollider.GetComponent<Rigidbody>();
        if (rb != null)
        {
            bool originalGravityState = rb.useGravity;

            if (enemyAI != null)
            {
                enemyAI.enabled = false;
            }

            rb.useGravity = true;

            yield return new WaitForSeconds(gravityDuration);

            rb.useGravity = false;

            if (enemyAI != null)
            {
                enemyAI.enabled = true;
            }
        }
    }

    private IEnumerator PlayAudioAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentDroppedObject != null)
        {
            AudioSource droppedAudioSource = currentDroppedObject.GetComponent<AudioSource>();
            if (droppedAudioSource != null)
            {
                droppedAudioSource.Play();
                yield return new WaitForSeconds(droppedAudioSource.clip.length);
                Destroy(currentDroppedObject, droppedAudioSource.clip.length + 3f);
            }
        }
    }

    private IEnumerator ReplenishCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(replenishCooldown);

        isObjectDropped = false;
        shieldsHit.Clear();
        isOnCooldown = false;
    }
}
















