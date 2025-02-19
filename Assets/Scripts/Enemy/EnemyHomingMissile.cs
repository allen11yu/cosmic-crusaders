using UnityEngine;

public class EnemyHomingMissile : MonoBehaviour
{
    private float currentSpeed = 0f;
    private float maxSpeed = 15f;
    private float rotationSpeed = 1f;
    private float acceleration = 2f;
    private IShootingStats shootingStats;
    private Transform target;
    private float spawnTime;
    private Rigidbody rb;

    public void Initialize(float initialSpeed, float maxSpeed, float acceleration, float rotationSpeed, Transform target, IShootingStats stats)
    {
        this.target = target;
        this.maxSpeed = maxSpeed;
        this.acceleration = acceleration;
        this.rotationSpeed = rotationSpeed;
        shootingStats = stats;
        spawnTime = Time.time;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on the missile.");
            return;
        }

        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * shootingStats.BulletSpeed;
        }
        else
        {
            Debug.LogWarning("No target found for missle.");
        }
        
        if (shootingStats == null)
        {
            Debug.LogError("ShootingStats not assigned to missile.");
            return;
        }

        currentSpeed = initialSpeed;
    }

    private void Update()
    {
        if (Time.time - spawnTime > shootingStats.BulletLifespan)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (target != null && rb != null)
        {
            // Rotate towards the target
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 rotateAmount = Vector3.Cross(transform.forward, direction);
            rb.angularVelocity = rotateAmount * rotationSpeed;

            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            rb.velocity = transform.forward * currentSpeed;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Only collide with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            player?.TakeDamage(shootingStats.Damage);

            // Destroy the missile after hitting the target
            Destroy(gameObject);
        }
    }
}
