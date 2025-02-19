using UnityEngine;

public class IncomingAsteroids : MonoBehaviour
{
    public GameObject[] asteroidPrefabs; 
    public float spawnDelay = 1f;        
    public int minAsteroids = 1;         
    public int maxAsteroids = 5;        
    public float spawnDistance = 15f;   
    public float spawnWidth = 8f;       
    public Camera playerCamera;          
    public float spawnHeight = 8f;     

    void Start()
    {
        InvokeRepeating(nameof(SpawnAsteroids), 0f, spawnDelay);
    }

    void SpawnAsteroids()
    {
        int asteroidCount = Random.Range(minAsteroids, maxAsteroids + 1);
        
        for (int i = 0; i < asteroidCount; i++)
        {
            SpawnAsteroid();
        }
    }

    void SpawnAsteroid()
    {
        Vector3 cameraPosition = playerCamera.transform.position;
        Vector3 spawnPosition = cameraPosition + playerCamera.transform.forward * spawnDistance;
        spawnPosition.y += spawnHeight;

        spawnPosition.x += Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        spawnPosition.y += Random.Range(-1f, 1f); 

        GameObject asteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
    
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            float forceMagnitude = Random.Range(5f, 15f); 
            rb.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
        }
    }
}
