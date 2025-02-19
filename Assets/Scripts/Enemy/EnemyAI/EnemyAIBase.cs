using UnityEngine;

public class EnemyAIBase : MonoBehaviour
{
    [Header("Prediction Settings")]
    public float lookaheadTime = 0.5f;


    protected Rigidbody rb;
    protected EnemyStats enemyStats;
    protected Transform playerTransform;

    protected float currentSpeed = 0f;
    protected float activeForwardSpeed;
    protected float currentBankAngle = 0f;
    protected float bankSmoothness = 2f;

    protected virtual void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        rb = GetComponent<Rigidbody>();

        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats component is missing on this GameObject.");
        }
    }

    protected virtual void FixedUpdate()
    {
        if (playerTransform == null || enemyStats == null)
            return;

        rb.angularVelocity = Vector3.zero;
    }

    // Override
    protected virtual void HandleAttack()
    {
        Debug.Log("Enemy is attacking");
    }

    protected bool IsFacingPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float alignment = Vector3.Dot(transform.forward, directionToPlayer);

        return alignment >= enemyStats.accuracy;
    }

    protected void HandleMovement(float targetSpeed)
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
        currentSpeed = Mathf.Clamp(currentSpeed, 0, enemyStats.speed);

        Vector3 movement = transform.forward * currentSpeed;
        rb.velocity = movement;
    }

    protected void HandleRotation(Vector3 targetDirection)
    {
        Vector3 direction = targetDirection.normalized;

        Vector3 localTargetDirection = transform.InverseTransformDirection(direction);
        float maxBankAngle = 30f;
        float bankAngle = Mathf.Atan2(localTargetDirection.x, localTargetDirection.z) * Mathf.Rad2Deg;
        bankAngle = Mathf.Clamp(bankAngle, -maxBankAngle, maxBankAngle);

        currentBankAngle = Mathf.Lerp(currentBankAngle, bankAngle, bankSmoothness * Time.fixedDeltaTime);

        Quaternion bankRotation = Quaternion.Euler(0, 0, -currentBankAngle);

        // Apply bank rotation first
        Quaternion targetRotation = Quaternion.LookRotation(direction) * bankRotation;

        float maxRotationAngle = enemyStats.lookRotateSpeed * Time.fixedDeltaTime;

        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationAngle);

        rb.MoveRotation(newRotation);
    }

    protected Vector3 PredictPlayerPosition()
    {
        Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
        if (playerRb)
        {
            Vector3 futurePosition = playerTransform.position + playerRb.velocity * this.lookaheadTime;
            return futurePosition;
        }

        return playerTransform.position;
    }
}