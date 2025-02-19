using UnityEngine;
using System.Collections;

public class FinalBossAI : EnemyAIBase
{
    [Header("Orbit Settings")]
    public Vector3 orbitCenter = Vector3.zero;
    public float orbitRadius = 50f;
    public float orbitSpeed = 20f;


    [Header("Basic Shooting Settings")]
    public float basicBulletSpeed = 30f;
    public float basicFireRate = 1f;


    [Header("Sniper Shooting Settings")]
    public float sniperChargeTime = 2f;
    public float sniperFireRate = 3f;


    [Header("Missile Shooting Settings")]
    public GameObject missilePrefab;
    public Transform missileShootOrigin;
    public int missilesPerVolley = 3;
    public float timeBetweenMissiles = 0.5f;
    public float missileRotationSpeed = 2f;
    public float missileAcceleration = 2f;
    public float missileMaxSpeed = 10f;
    public float missileLifespan = 5f;
    public float missileReloadTime = 7f;


    private float nextBasicFireTime = 0f;
    private float nextSniperFireTime = 0f;
    private float nextMissileFireTime = 0f;

    private float missilesFired = 0f;

    private EnemyStats bossStats;

    private bool isSniperCharging = false;
    private EnemyShooting enemyShooting;

    protected override void Start()
    {
        base.Start();

        enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting == null)
        {
            Debug.LogError("EnemyShooting component is missing on this GameObject.");
        }

        bossStats = GetComponent<EnemyStats>();
        if (bossStats == null)
        {
            Debug.LogError("EnemyStats component is missing on the Final Boss.");
        }

        Vector3 initialDirection = (transform.position - orbitCenter).normalized;
        transform.position = orbitCenter + initialDirection * orbitRadius;
    }

    void Update()
    {
        if (nextSniperFireTime > 0f)
        {
            nextSniperFireTime -= Time.deltaTime;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        HandleOrbitMovement();
        HandleShootingPatterns();
    }

    private void HandleOrbitMovement()
    {
        // Correctly calculate the direction towards the orbit center
        Vector3 directionToCenter = (orbitCenter - transform.position).normalized;

        // Calculate the perpendicular direction for orbiting
        Vector3 orbitDirection = Vector3.Cross(Vector3.up, directionToCenter).normalized;

        // Smoothly interpolate the current speed towards the desired speed
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);

        // Move the boss forward in the orbit direction
        base.HandleMovement(activeForwardSpeed);

        // Rotate the boss to face the orbit direction
        base.HandleRotation(orbitDirection);
    }

    private void HandleShootingPatterns()
    {
        float healthPercentage = ((float)bossStats.currentHealth / bossStats.maxHealth) * 100f;

        if (healthPercentage >= 75f)
        {
            if (Time.time >= nextBasicFireTime)
            {
                Vector3 direction = (base.PredictPlayerPosition() - enemyShooting.shootOrigin.position).normalized;
                enemyShooting.Shoot(null, direction);
                nextBasicFireTime = Time.time + basicFireRate;
            }
        }
        else if (healthPercentage >= 50f)
        {
            if (!isSniperCharging && nextSniperFireTime <= 0f)
            {
                // Calculate required bullet speed to intercept player
                float distanceToPredicted = Vector3.Distance(transform.position, base.PredictPlayerPosition());
                float requiredBulletSpeed = distanceToPredicted / base.lookaheadTime;
                StartCoroutine(ChargeAndShootSniper(requiredBulletSpeed));
            }
        }
        else if (healthPercentage >= 25f)
        {
            if (missilesFired < missilesPerVolley)
            {
                nextMissileFireTime -= Time.deltaTime;
                if (nextMissileFireTime <= 0f)
                {
                    FireMissile();
                    missilesFired++;
                    nextMissileFireTime = timeBetweenMissiles;
                }
            }
            else
            {
                nextMissileFireTime -= Time.deltaTime;
                if (nextMissileFireTime <= 0f)
                {
                    missilesFired = 0;
                    nextMissileFireTime = missileReloadTime;
                }
            }
        }
        else
        {
            if (Time.time >= nextBasicFireTime)
            {
                Debug.Log("Boss is firing");

                Vector3 direction = (base.PredictPlayerPosition() - enemyShooting.shootOrigin.position).normalized;
                enemyShooting.Shoot(null, direction);
                nextBasicFireTime = Time.time + basicFireRate;
            }

            if (!isSniperCharging && nextSniperFireTime <= 0f)
            {
                // Calculate required bullet speed to intercept player
                float distanceToPredicted = Vector3.Distance(transform.position, base.PredictPlayerPosition());
                float requiredBulletSpeed = distanceToPredicted / base.lookaheadTime;
                StartCoroutine(ChargeAndShootSniper(requiredBulletSpeed));
            }

            if (missilesFired < missilesPerVolley)
            {
                nextMissileFireTime -= Time.deltaTime;
                if (nextMissileFireTime <= 0f)
                {
                    FireMissile();
                    missilesFired++;
                    nextMissileFireTime = timeBetweenMissiles;
                }
            }
            else
            {
                nextMissileFireTime -= Time.deltaTime;
                if (nextMissileFireTime <= 0f)
                {
                    missilesFired = 0;
                    nextMissileFireTime = missileReloadTime;
                }
            }
        }
    }

    private IEnumerator ChargeAndShootSniper(float sniperBulletSpeed)
    {
        Debug.Log("Boss is sniping");

        isSniperCharging = true;

        yield return new WaitForSeconds(sniperChargeTime);

        Vector3 direction = (PredictPlayerPosition() - enemyShooting.shootOrigin.position).normalized;
        enemyShooting.Shoot(sniperBulletSpeed, direction);

        nextSniperFireTime = base.enemyStats.fireRate;
        isSniperCharging = false;
    }

    private void FireMissile()
    {
        Debug.Log("Boss is firing missles");

        if (missilePrefab == null || base.playerTransform == null)
        {
            Debug.LogWarning("MissilePrefab or Target is not assigned.");
            return;
        }

        GameObject missile = Instantiate(missilePrefab, missileShootOrigin.position, missileShootOrigin.rotation);

        EnemyHomingMissile homingMissile = missile.GetComponent<EnemyHomingMissile>();
        if (homingMissile != null)
        {
            homingMissile.Initialize(
                initialSpeed: rb.velocity.magnitude,
                maxSpeed: missileMaxSpeed,
                acceleration: missileAcceleration,
                rotationSpeed: missileRotationSpeed,
                target: playerTransform,
                stats: enemyStats
            );
        }
        else
        {
            Debug.LogError("EnemyHomingMissile component not found on the missile prefab.");
        }
    }
}
