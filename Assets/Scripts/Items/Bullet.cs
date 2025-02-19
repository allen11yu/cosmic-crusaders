using UnityEngine;

public class Bullet : MonoBehaviour
{
    private IShootingStats shootingStats;

    private float spawnTime;

    public void Initialize(Vector3 shooterDirection, Vector3 shooterVelocity, IShootingStats stats, float? bulletSpeed = null)
    {
        shootingStats = stats;
        spawnTime = Time.time;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (bulletSpeed != null)
            {
                rb.velocity = (Vector3)(shooterDirection.normalized * bulletSpeed);
            }
            else
            {
                rb.velocity = shooterDirection.normalized * stats.BulletSpeed + shooterVelocity;
            }
        }
    }

    void Update()
    {
        if (shootingStats == null)
        {
            Debug.LogError("ShootingStats not assigned to bullet.");
            return;
        }

        if (Time.time - spawnTime > shootingStats.BulletLifespan)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (gameObject.CompareTag("PlayerBullet") && (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Asteroid") || collision.CompareTag("FinalBoss")))
        {
            EnemyStats enemy = collision.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(shootingStats.Damage);
            }

            AsteroidDestroy asteroid = collision.GetComponent<AsteroidDestroy>();
            if (asteroid != null)
            {
                asteroid.DestroyAsteroid();
            }

            ReturnToPool();
        }
        else if (gameObject.CompareTag("EnemyBullet") && collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(shootingStats.Damage);
            }
        }
    }

    void ReturnToPool()
    {
        // Return to the appropriate pool based on bulletType
        if (gameObject.CompareTag("PlayerBullet") && PlayerBulletPool.Instance != null)
        {
            PlayerBulletPool.Instance.ReturnBullet(this.gameObject);
        }
        else if (gameObject.CompareTag("EnemyBullet") && EnemyBulletPool.Instance != null)
        {
            EnemyBulletPool.Instance.ReturnBullet(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Bullet type not recognized or pool instance missing. Destroying bullet.");
            Destroy(gameObject);
        }
    }
}
