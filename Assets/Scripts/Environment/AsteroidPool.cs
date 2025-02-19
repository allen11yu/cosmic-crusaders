using System.Collections.Generic;
using UnityEngine;

public class AsteroidPool : MonoBehaviour
{
    public static AsteroidPool Instance;

    [Header("Pool Configuration")]
    public List<GameObject> asteroidPrefabs;
    public int asteroidPoolSize = 100;

    private Queue<GameObject> asteroidPool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePools()
    {
        for (int i = 0; i < asteroidPoolSize; i++)
        {
            // randomly select asteroid
            GameObject randomAsteroidPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Count)];
            GameObject asteroid = Instantiate(randomAsteroidPrefab);
            asteroid.SetActive(false);
            asteroidPool.Enqueue(asteroid);
        }
    }

    public GameObject GetAsteroid()
    {
        GameObject asteroid;

        if (asteroidPool.Count > 0)
        {
            asteroid = asteroidPool.Dequeue();
            asteroid.SetActive(true);
            
            // golden asteroids
            // Renderer renderer = asteroid.GetComponent<Renderer>();
            // if (renderer != null)
            // {
            //     if (isHealthPack && Random.value < 0.25f)
            //     {
            //         Color goldColor = new Color(1.0f, 0.84f, 0.0f);
            //         renderer.material.color = goldColor;

            //         renderer.material.EnableKeyword("_EMISSION");
            //         renderer.material.SetColor("_EmissionColor", goldColor * 0.5f);

            //         Debug.Log($"Temp boost asteroid generated.");
            //     }
            // }

            return asteroid;
        }

        return null;
    }

    public void ReturnAsteroid(GameObject asteroid)
    {
        asteroid.SetActive(false);
        asteroidPool.Enqueue(asteroid);
    }
}
