using UnityEngine;

public class ArtilleryEnemyAI : EnemyAIBase
{
    public enum EnemyState
    {
        Approaching,
        Attacking,
        Retreating
    }

    [Header("Missile Settings")]
    public GameObject missilePrefab;
    public Transform missileSpawnPoint;
    public int missilesPerVolley = 4;
    
    // Time delay between each missile in a volley
    public float timeBetweenMissiles = 0.5f;
    public float missileRotationSpeed = 5f;
    public float missileAcceleration = 2f;
    public float missileMaxSpeed = 30f;


    [Header("Engagement Settings")]
    public float preferredDistance = 100f;
    public float minEngagementDistance = 25f;
    public float maxEngagementDistance = 150f;


    [Header("Reload Settings")]
    public float reloadTime = 7f;


    private EnemyState currentState = EnemyState.Approaching;
    private float stateTimer = 0f;
    private int missilesFired = 0;

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Approaching:
                HandleApproach();
                break;
            case EnemyState.Attacking:
                HandleAttack();
                break;
            case EnemyState.Retreating:
                HandleRetreat();
                break;
        }
    }

    protected override void HandleAttack()
    {
        Debug.Log("Artillery enemy is Attacking");

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxEngagementDistance)
        {
            currentState = EnemyState.Approaching;
            return;
        }
        else if (distanceToPlayer < minEngagementDistance)
        {
            currentState = EnemyState.Retreating;
            return;
        }
        PerformOrbitMovement();

        if (missilesFired < missilesPerVolley)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                FireMissile();
                missilesFired++;
                stateTimer = timeBetweenMissiles;
            }
        }
        else
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                missilesFired = 0;
                stateTimer = reloadTime;
            }
        }
    }

    private void HandleApproach()
    {
        Debug.Log("Artillery enemy is approaching");

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= preferredDistance)
        {
            currentState = EnemyState.Attacking;
            return;
        }

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
        HandleMovement(activeForwardSpeed);

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        HandleRotation(directionToPlayer);
    }

    private void PerformOrbitMovement()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 orbitDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed, enemyStats.forwardAcceleration * Time.deltaTime);
        HandleMovement(activeForwardSpeed);

        HandleRotation(orbitDirection);
    }

    private void HandleRetreat()
    {
        Debug.Log("Artillery enemy is retreating");
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer >= preferredDistance)
        {
            currentState = EnemyState.Attacking;
            return;
        }

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed * 1.5f, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
        HandleMovement(activeForwardSpeed);

        Vector3 directionAwayFromPlayer = (transform.position - playerTransform.position).normalized;
        HandleRotation(directionAwayFromPlayer);
    }

    private void FireMissile()
    {
        Debug.Log("Artillery enemy is firing");

        if (missilePrefab == null || base.playerTransform == null)
        {
            Debug.LogWarning("MissilePrefab or Target is not assigned.");
            return;
        }

        GameObject missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);

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
