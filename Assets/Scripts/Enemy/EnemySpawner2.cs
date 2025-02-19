using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawner2 : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject commonEnemyPrefab;
    public GameObject unmannedEnemyPrefab;
    public GameObject sniperEnemyPrefab;
    public GameObject shieldEnemyPrefab;
    public GameObject artilleryEnemyPrefab;
    public GameObject bossEnemyPrefab;

    [Header("Boundary Settings")]
    public Transform sphericalBoundary;
    public float spawnDelay = 15f;

    [Header("Info Card Settings")]
    public GameObject enemyInfoCard;
    public TextMeshProUGUI enemyInfoText;
    public float infoCardDuration = 5f;

    [Header("Wave UI Settings")]
    public TextMeshProUGUI waveText;

    [Header("Crosshair Settings")]
    public Camera mainCamera;
    public float sphereCastRadius = 5f;
    public float sphereCastDistance = 100f;
    public CrosshairController crosshairController;

    private int currentRound = 1;
    private bool countdownStarted = false;
    private KeyCode nextRoundKey = KeyCode.N;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public AudioClip bossSpawnSound;
    private AudioSource audioSource;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not assigned and could not find Camera.main!");
            }
        }

        if (enemyInfoCard != null)
        {
            enemyInfoCard.SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (waveText != null)
        {
            waveText.text = "Wave " + currentRound;
        }

        StartSpawning();
    }

    void Update()
    {
        // Check if we need to start the next round after all enemies are defeated
        if (countdownStarted && Input.GetKeyDown(nextRoundKey) && AllEnemiesDefeated())
        {
            StopAllCoroutines();
            StartCoroutine(StartNextRound());
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnWave());
    }

    private bool AllEnemiesDefeated()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null) // If any enemy is still active, return false
            {
                return false;
            }
        }
        return true; // If no active enemies, return true
    }

    private IEnumerator SpawnWave()
    {
        // Clear previously spawned enemies before starting a new wave
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();

        ShowWaveNumber(currentRound);

        // Spawn enemies based on the current wave
        switch (currentRound)
        {
            case 1:
                SpawnEnemy(commonEnemyPrefab);
                break;

            case 2:
                SpawnEnemy(commonEnemyPrefab);
                SpawnEnemy(unmannedEnemyPrefab);
                SpawnEnemy(unmannedEnemyPrefab);
                break;

            case 3:
                SpawnEnemy(commonEnemyPrefab);
                SpawnEnemy(commonEnemyPrefab);
                SpawnEnemy(commonEnemyPrefab);
                SpawnEnemy(shieldEnemyPrefab);
                break;

            case 4:
                SpawnEnemy(commonEnemyPrefab);
                SpawnEnemy(shieldEnemyPrefab);
                SpawnEnemy(sniperEnemyPrefab);
                SpawnEnemy(sniperEnemyPrefab);
                break;

            case 5:
                SpawnEnemy(shieldEnemyPrefab);
                SpawnEnemy(sniperEnemyPrefab);
                SpawnEnemy(artilleryEnemyPrefab);
                SpawnEnemy(artilleryEnemyPrefab);
                break;

            case 6:
                SpawnEnemy(bossEnemyPrefab);
                break;
        }

        // Wait until all enemies are defeated
        yield return new WaitUntil(() => AllEnemiesDefeated());

        countdownStarted = true;
        yield return StartCoroutine(WaitForNextRoundOrSkip());
        countdownStarted = false;

        // Proceed to next wave only if we're not at the final wave
        if (currentRound < 6)
        {
            currentRound++;
            StartCoroutine(SpawnWave());
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        float radius = sphericalBoundary.localScale.x * 0.5f;

        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 spawnPosition = sphericalBoundary.position + randomDirection * radius;

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.LookRotation(-randomDirection));
        spawnedEnemies.Add(spawnedEnemy);

        if (enemyPrefab == bossEnemyPrefab && bossSpawnSound != null)
        {
            audioSource.PlayOneShot(bossSpawnSound);
        }

        EnemyIndicatorManager indicatorManager = FindObjectOfType<EnemyIndicatorManager>();
        if (indicatorManager != null)
        {
            indicatorManager.RegisterEnemy(spawnedEnemy);
        }
    }

    private IEnumerator StartNextRound()
    {
        // Clear the list and reset flag for the next round
        spawnedEnemies.Clear();
        yield return SpawnWave(); // Continue with the new wave
    }

    private IEnumerator WaitForNextRoundOrSkip()
    {
        ShowWaveNumber(currentRound);

        float elapsed = 0f;
        while (elapsed < spawnDelay)
        {
            if (Input.GetKeyDown(nextRoundKey)) // Allow skipping of the delay
            {
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void ShowWaveNumber(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + waveNumber;
        }
    }
}
