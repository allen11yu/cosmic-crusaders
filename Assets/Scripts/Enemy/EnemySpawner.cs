using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class EnemySpawner : MonoBehaviour
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
    public GameObject enemyInfoCard; // Reference to the Enemy Info Card UI
    public TextMeshProUGUI enemyInfoText; // Reference to the Text in the Info Card
    public float infoCardDuration = 3f; // Duration to show the info card


    [Header("Crosshair Settings")]
    public Camera mainCamera;
    public float sphereCastRadius = 5f; // Radius of the SphereCast for enemy detection
    public float sphereCastDistance = 100f; // Maximum distance of the SphereCast


    [Header("Wave")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyLeftText;
    public WaveUI waveX;


    public CrosshairController crosshairController; // Reference to the CrosshairController
    public float crosshairProximityRadius = 100f; // Radius around the crosshair to detect enemies
    public float crosshairPadding = 10f; // Add padding around the crosshair bounds

    public int[] enemiesPerWave = { 1, 2, 3, 3, 4 }; // Number of enemies in each wave
    public float timeBetweenWaves = 5f; // Delay between waves
    public int currentWave = 0;
    private int n_enemies = 0;

    //private KeyCode nextWaveKey = KeyCode.N;
    //private bool waitingForNextWave = false;
    private bool allEnemiesDefeated = false;
    

    public AudioClip bossSpawnSound;
    private AudioSource audioSource;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private HashSet<string> displayedEnemyTypes = new HashSet<string>();


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
            enemyInfoCard.SetActive(false); // Ensure the info card is hidden initially
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        StartSpawning();
    }

    void Update()
    {
        CheckEnemiesInCrosshair();
        n_enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            allEnemiesDefeated = true;
        }

        string waveString = string.Format("Wave: {0:F0}", currentWave);
        waveText.text = waveString;

        string nEnemyLeft = string.Format("Number of enemies left: {0:F0}", n_enemies);
        enemyLeftText.text = nEnemyLeft;
    }

    public void StartSpawning()
    {
        //StartCoroutine(SpawnRound());
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWave < enemiesPerWave.Length)
        {
            waveX.ShowWaveText(currentWave);
            yield return StartCoroutine(SpawnWave(currentWave));
            currentWave++;
        }
    }

    private IEnumerator SpawnWave(int waveIndex)
    {
        int numEnemies;
        if (waveIndex == 0)
        {
            numEnemies = enemiesPerWave[waveIndex];
        }
        else
        {
            numEnemies = Random.Range(1, 4);
        }
        List<GameObject> spawnedThisWave = new List<GameObject>();

        if (waveIndex == enemiesPerWave.Length - 1)
        {
            // Spawn the boss
            SpawnEnemy(bossEnemyPrefab);

            // Spawn additional random enemies
            for (int i = 0; i < numEnemies; i++)
            {
                GameObject[] possibleEnemies = {
                commonEnemyPrefab,
                unmannedEnemyPrefab,
                sniperEnemyPrefab,
                artilleryEnemyPrefab
                };

                int randomEnemyIndex = Random.Range(0, possibleEnemies.Length);
                GameObject enemyPrefab = possibleEnemies[randomEnemyIndex];

                SpawnEnemy(enemyPrefab);
                spawnedThisWave.Add(enemyPrefab);
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            bool shieldEnemySpawned = false;

            for (int i = 0; i < numEnemies; i++)
            {
                GameObject enemyPrefab;
                do
                {
                    GameObject[] possibleEnemies = {
                    commonEnemyPrefab,
                    unmannedEnemyPrefab,
                    sniperEnemyPrefab,
                    shieldEnemyPrefab,
                    artilleryEnemyPrefab
                    };
                    // Random Enemy to be spawned
                    int randomEnemyIndex = Random.Range(0, possibleEnemies.Length);
                    enemyPrefab = possibleEnemies[randomEnemyIndex];

                } while (spawnedThisWave.Contains(enemyPrefab) ||
                         (enemyPrefab == shieldEnemyPrefab && shieldEnemySpawned));

                // Only spawn one shield enemy per round
                if (enemyPrefab == shieldEnemyPrefab)
                {
                    shieldEnemySpawned = true;
                }

                SpawnEnemy(enemyPrefab);
                spawnedThisWave.Add(enemyPrefab);
                yield return new WaitForSeconds(1f);
            }
        }

        yield return new WaitUntil(() => allEnemiesDefeated);
        allEnemiesDefeated = false;
        yield return new WaitForSeconds(timeBetweenWaves);
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        float radius = sphericalBoundary.localScale.x * 0.5f;

        // Random point on the edge of boundary
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 spawnPosition = sphericalBoundary.position + randomDirection * radius;

        // Spawn the enemy at random point
        // Instantiate(enemyPrefab, spawnPosition, Quaternion.LookRotation(-randomDirection));
        // Spawn the enemy at random point
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.LookRotation(-randomDirection));
        spawnedEnemies.Add(spawnedEnemy);

        if (enemyPrefab == bossEnemyPrefab && bossSpawnSound != null)
        {
            audioSource.PlayOneShot(bossSpawnSound);
        }

        // Register the enemy with the indicator manager
        EnemyIndicatorManager indicatorManager = FindObjectOfType<EnemyIndicatorManager>();
        if (indicatorManager != null)
        {
            indicatorManager.RegisterEnemy(spawnedEnemy);
        }
    }


    private void CheckEnemiesInCrosshair()
    {
        if (mainCamera == null) return;

        // Define the origin and direction of the SphereCast
        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        // Perform the SphereCast
        RaycastHit hit;
        bool hitDetected = Physics.SphereCast(origin, sphereCastRadius, direction, out hit, sphereCastDistance);

        if (hitDetected)
        {
            // Check if the hit object is tagged as "Enemy"
            if (hit.collider.CompareTag("Enemy"))
            {
                // Get the enemy's world position
                Vector3 enemyWorldPosition = hit.collider.transform.position;

                // Use the crosshair controller to determine if the enemy is in the crosshair bounds
                if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("FinalBoss"))
                {
                    Debug.Log("Enemy detected in crosshair via SphereCast and confirmed by crosshair bounds!");

                    // Determine the enemy type and show the appropriate message
                    GameObject enemy = hit.collider.gameObject;

                    string enemyMessage;
                    string enemyTypeKey = "";

                    switch (enemy.name)
                    {
                        case string name when name.Contains(commonEnemyPrefab.name):
                            enemyMessage = "Common Enemy: A straightforward foe. No special tricks, just strength in numbers.";
                            enemyTypeKey = "CommonEnemy";

                            break;
                        case string name when name.Contains(shieldEnemyPrefab.name):
                            enemyMessage = "Shield Generator: Protects nearby allies with a powerful energy shield. Take it out first!";
                            enemyTypeKey = "ShieldGenerator";
                            break;
                        case string name when name.Contains(unmannedEnemyPrefab.name):
                            enemyMessage = "Drone: A relentless kamikaze attacker. Quick and lethal on impact!";
                            enemyTypeKey = "Drone";
                            break;
                        case string name when name.Contains(sniperEnemyPrefab.name):
                            enemyMessage = "Sniper: A sharpshooter lurking in the distance. Dodge its deadly long-range shots!";
                            enemyTypeKey = "Sniper";
                            break;
                        case string name when name.Contains(artilleryEnemyPrefab.name):
                            enemyMessage = "Artillery: A slow-moving powerhouse firing devastating explosive rockets. Stay out of its blast radius!";
                            enemyTypeKey = "Artillery";
                            break;
                        case string name when name.Contains(bossEnemyPrefab.name):
                            enemyMessage = "Boss Enemy: A juggernaut of destruction. Rockets, shields, ramming — it has everything. Brace yourself!";
                            enemyTypeKey = "Boss";
                            break;
                        default:
                            enemyMessage = "Unknown Enemy: Beware! It could be a new threat.";
                            enemyTypeKey = "Unknown";
                            break;
                    }


                    // Show the enemy info card with the appropriate message
                    // ShowEnemyInfoCard(enemyMessage, enemy);
                    // Check if this enemy type has already been displayed
                    if (!displayedEnemyTypes.Contains(enemyTypeKey))
                    {
                        // If not displayed, show the info card and add to the HashSet
                        ShowEnemyInfoCard(enemyMessage, enemy);
                        displayedEnemyTypes.Add(enemyTypeKey);
                    }
                }
                else
                {
                    Debug.Log("Enemy detected by SphereCast but not within crosshair bounds.");
                }
            }
        }
    }

    private void ShowEnemyInfoCard(string message, GameObject enemy)
    {
        if (enemyInfoCard != null && enemyInfoText != null)
        {
            enemyInfoText.text = message;
            enemyInfoCard.SetActive(true);
            CancelInvoke(nameof(HideEnemyInfoCard));
            Invoke(nameof(HideEnemyInfoCard), infoCardDuration);
        }
    }

    private void HideEnemyInfoCard()
    {
        if (enemyInfoCard != null && enemyInfoCard.activeSelf)
        {
            enemyInfoCard.SetActive(false); // Hide the info card
        }
    }

}
