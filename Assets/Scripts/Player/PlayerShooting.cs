using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform shootOrigin;


    [Header("Audio Sound")]
    public AudioClip shootSound;


    private float nextFireTime = 0f;

    private Rigidbody rb;
    private PlayerStats playerStats;
    private AudioSource audioSource;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats  == null)
        {
            Debug.LogError("PlayerShooting needs a ShootingStats component");
        }

        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("PlayerShooting needs a RigidBody");
        }

        if (shootOrigin == null)
        {
            Debug.LogError("PlayerShooting needs a shoot origin Transform");
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + playerStats.FireRate;
        }
    }

    void Shoot()
    {
        if (PlayerBulletPool.Instance == null)
        {
            Debug.LogError("BulletPool not found");
            return;
        }

        // Get a bullet from the pool
        GameObject bullet = PlayerBulletPool.Instance.GetBullet();
        if (bullet == null)
        {
            Debug.LogWarning("No bullets in pool");
            return;
        }

        // Set bullet position and rotation
        bullet.transform.position = shootOrigin.position;
        bullet.transform.rotation = shootOrigin.rotation;

        // Initialize bullet
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(shootOrigin.forward, rb.velocity, playerStats);
        }
        else
        {
            Debug.LogError("No Bullet script on bullet prefab");
        }

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
