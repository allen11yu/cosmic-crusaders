using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Boundary Sphere")]
    public Transform BoundarySphere;
    public GameObject player;


    [Header("Asteroid Spawn Settings")]
    public float spawnInterval = 0.1f;
    public float minY = -50;
    public float maxY = 100;
    public float scale;


    private float boundaryRadius;

    private void Start()
    {
        Debug.Log("AsteroidSpawner started.");
        InvokeRepeating(nameof(SpawnAsteroid), 0, spawnInterval);

        boundaryRadius = BoundarySphere.localScale.x * 0.5f;
    }

    void SpawnAsteroid()
    {
        Vector3 randomPosition = GetRandomPosition();

        GameObject asteroid = AsteroidPool.Instance.GetAsteroid();
        if (asteroid == null)
        {
            return;
        }
        asteroid.transform.position = randomPosition;

        // Randomly scaling of big and small asteroid.
        scale = Random.Range(0.5f, 2.0f);
        asteroid.transform.localScale = new Vector3(scale, scale, scale);
    }

    private Vector3 GetRandomPosition()
    {
        // compute random position within the map boundary
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(0f, boundaryRadius);

        float xPos = player.transform.position.x + distance * Mathf.Cos(angle);
        float yPos = Random.Range(minY, maxY);
        float zPos = player.transform.position.z + distance * Mathf.Sin(angle);

        return new Vector3(xPos, yPos, zPos);
    }
}
