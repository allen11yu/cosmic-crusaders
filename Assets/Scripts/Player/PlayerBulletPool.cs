using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletPool : MonoBehaviour
{
    public static PlayerBulletPool Instance;

    [Header("Pool Configuration")]
    public GameObject playerBulletPrefab;
    public int poolSize = 100;

    private Queue<GameObject> playerBulletPool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(playerBulletPrefab);
            bullet.SetActive(false);
            playerBulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (playerBulletPool.Count > 0)
        {
            GameObject bullet = playerBulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(playerBulletPrefab);
            return bullet;
        }
    }
    
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        playerBulletPool.Enqueue(bullet);
    }
}
