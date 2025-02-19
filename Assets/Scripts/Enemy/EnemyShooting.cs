using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [Header("Shooting Configuration")]
    public GameObject bulletPrefab;
    public Transform shootOrigin;

    private EnemyStats enemyStats;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats component is missing on this GameObject.");
        }

        if (shootOrigin == null)
        {
            Debug.LogError("EnemyShooting needs a shoot origin Transform");
        }
    }

    public void Shoot(float? bulletSpeed = null, Vector3? shootDirection = null)
    {
        if (EnemyBulletPool.Instance == null)
        {
            Debug.LogError("BulletPool not found");
            return;
        }

        // Get a bullet from the pool
        GameObject bullet = EnemyBulletPool.Instance.GetBullet();
        if (bullet == null)
        {
            Debug.LogWarning("No bullets in pool");
            return;
        }

        // Set bullet position and rotation
        bullet.transform.position = shootOrigin.position;

        Vector3 finalShootDirection = shootDirection ?? shootOrigin.forward;
        bullet.transform.rotation = Quaternion.LookRotation(finalShootDirection);

        // Initialize bullet
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(finalShootDirection, rb.velocity, enemyStats, bulletSpeed);
        }
        else
        {
            Debug.LogError("No Bullet script on bullet prefab");
        }
    }
}
