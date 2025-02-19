using System.Collections;
using UnityEngine;

public class SniperEnemyAI : EnemyAIBase
{
    [Header("Sniper AI Settings")]
    public float chaseRange = 50f;
    public float chargeTime = 3f;


    private bool isCharging = false;
    private float fireCooldownTimer = 0f;

    private EnemyShooting enemyShooting;

    protected override void Start()
    {
        base.Start();

        enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting == null)
        {
            Debug.LogError("EnemyShooting component is missing on this GameObject.");
        }
    }

    void Update()
    {
        if (fireCooldownTimer > 0f)
        {
            fireCooldownTimer -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > chaseRange)
        {
            HandleApproach();
        }
        else
        {
            HandleAttack();
        }
    }

    private void HandleApproach()
    {
        Debug.Log("Sniper enemy is approaching");

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
        HandleMovement(activeForwardSpeed);

        Vector3 directionToPredicted = (base.PredictPlayerPosition() - transform.position).normalized;
        HandleRotation(directionToPredicted);
    }

    protected override void HandleAttack()
    {
        Debug.Log("Sniper enemy is sniping");

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, 0f, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
        HandleMovement(activeForwardSpeed);

        Vector3 directionToPredicted = (base.PredictPlayerPosition() - transform.position).normalized;
        HandleRotation(directionToPredicted);

        if (!isCharging && fireCooldownTimer <= 0f)
        {
            // Calculate required bullet speed to intercept player
            float distanceToPredicted = Vector3.Distance(transform.position, base.PredictPlayerPosition());
            float requiredBulletSpeed = distanceToPredicted / base.lookaheadTime;
            StartCoroutine(ChargeAndShoot(requiredBulletSpeed));
        }
    }

    private IEnumerator ChargeAndShoot(float bulletSpeed)
    {
        isCharging = true;
        // ShowChargeEffect();

        yield return new WaitForSeconds(chargeTime);
        enemyShooting.Shoot(bulletSpeed);

        fireCooldownTimer = base.enemyStats.fireRate;
        isCharging = false;
    }
}
