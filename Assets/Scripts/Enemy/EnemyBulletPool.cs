using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletPool : MonoBehaviour
{
    public static EnemyBulletPool Instance;

    [Header("Pool Configuration")]
    public GameObject enemyBulletPrefab;
    public int poolSize = 300;

    private Queue<GameObject> enemyBulletPool = new Queue<GameObject>();

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
            GameObject bullet = Instantiate(enemyBulletPrefab);
            bullet.SetActive(false);
            enemyBulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (enemyBulletPool.Count > 0)
        {
            GameObject bullet = enemyBulletPool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            GameObject bullet = Instantiate(enemyBulletPrefab);
            return bullet;
        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        enemyBulletPool.Enqueue(bullet);
    }
}
