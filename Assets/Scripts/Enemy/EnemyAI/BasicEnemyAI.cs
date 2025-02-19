using UnityEngine;

public class BasicEnemyAI : EnemyAIBase
{
    public enum EnemyState
    {
        Attack,
        Evade
    }
    public enum EvadeManeuver
    {
        None,
        BarrelRoll,
        SharpTurn,
        VerticalLoop,
        ZigZag
    }

    private EnemyState currentState;
    private EvadeManeuver currentManeuver = EvadeManeuver.None;

    private float maneuverDuration = 0f;
    private float maneuverTimer = 0f;
    private float nextFireTime = 0f;
    private EnemyShooting enemyShooting;

    protected override void Start()
    {
        base.Start();

        enemyShooting = GetComponent<EnemyShooting>();
        if (enemyShooting == null)
        {
            Debug.LogError("EnemyShooting component is missing on this GameObject.");
        }

        base.enemyStats.DamageTaken += TriggerEvasion;

        currentState = EnemyState.Attack;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        switch (currentState)
        {
            case EnemyState.Attack:
                HandleAttack();
                break;
            case EnemyState.Evade:
                HandleEvade();
                break;
        }
    }

    void Update()
    {
        if (Time.time >= nextFireTime && IsFacingPlayer())
        {
            enemyShooting.Shoot();
            nextFireTime = Time.time + base.enemyStats.fireRate;
        }
    }

    protected override void HandleAttack()
    {
        Debug.Log("Enemy is starting attack");
        Vector3 predictedPosition = base.PredictPlayerPosition();

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
        HandleMovement(activeForwardSpeed);

        Vector3 directionToPredicted = (predictedPosition - transform.position).normalized;
        HandleRotation(directionToPredicted);
    }

    private void StartEvade()
    {
        Debug.Log("Enemy is starting evasion");

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.boostSpeed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);

        int maneuverIndex = Random.Range(0, 4);
        switch (maneuverIndex)
        {
            case 0:
                currentManeuver = EvadeManeuver.BarrelRoll;
                break;
            case 1:
                currentManeuver = EvadeManeuver.SharpTurn;
                break;
            case 2:
                currentManeuver = EvadeManeuver.VerticalLoop;
                break;
            case 3:
                currentManeuver = EvadeManeuver.ZigZag;
                break;
        }

        currentState = EnemyState.Evade;
    }

    private void HandleEvade()
    {
        maneuverTimer += Time.fixedDeltaTime;

        switch (currentManeuver)
        {
            case EvadeManeuver.BarrelRoll:
                PerformBarrelRoll();
                break;

            case EvadeManeuver.SharpTurn:
                PerformSharpTurn();
                break;

            case EvadeManeuver.VerticalLoop:
                PerformVerticalLoop();
                break;

            case EvadeManeuver.ZigZag:
                PerformRandomZigzag();
                break;
        }

        if (maneuverTimer >= maneuverDuration)
        {
            currentState = EnemyState.Attack;
            currentManeuver = EvadeManeuver.None;
        }

        base.HandleMovement(activeForwardSpeed);
    }


    private void TriggerEvasion()
    {
        if (maneuverTimer <= 0f)
        {
            StartEvade();
        }
    }

    private void PerformBarrelRoll()
    {
        Debug.Log("BarrelRoll");
        float direction = 1f;

        // Perform the roll
        float rollAngle = base.enemyStats.rollSpeed * 3 * Time.fixedDeltaTime * direction;
        Quaternion rollRotation = Quaternion.Euler(0, 0, rollAngle);
        rb.MoveRotation(rb.rotation * rollRotation);
    }

    private void PerformSharpTurn()
    {
        Debug.Log("Sharp turn");
        float turnSpeed = base.enemyStats.lookRotateSpeed * 2f;
        float direction = 1f;

        float turnAngle = turnSpeed * Time.fixedDeltaTime * direction;
        Quaternion turnRotation = Quaternion.Euler(0, turnAngle, 0);
        rb.MoveRotation(base.rb.rotation * turnRotation);
    }

    private void PerformVerticalLoop()
    {
        Debug.Log("Loop");
        float loopSpeed = 360f;
        float direction = 1f;

        // Perform the loop
        float loopAngle = loopSpeed * Time.fixedDeltaTime * direction;
        Quaternion loopRotation = Quaternion.Euler(loopAngle, 0, 0);
        rb.MoveRotation(loopRotation * base.rb.rotation);
    }

    private void PerformRandomZigzag()
    {
        Debug.Log("ZigZag");
        float zigzagFrequency = .25f;
        float zigzagAmplitude = base.enemyStats.lookRotateSpeed;
        float direction = Mathf.Sin(2 * Mathf.PI * zigzagFrequency * maneuverTimer) > 0 ? 1f : -1f;

        float turnAngle = zigzagAmplitude * Time.fixedDeltaTime * direction;
        Quaternion turnRotation = Quaternion.Euler(0, turnAngle, 0);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}