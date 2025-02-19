using UnityEngine;

public class UnmannedEnemyAI : EnemyAIBase
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        HandleAttack();
    }

    protected override void HandleAttack()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        currentSpeed = Mathf.Lerp(GetCurrentSpeed(), base.currentSpeed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);

        base.HandleMovement(currentSpeed);
        base.HandleRotation(directionToPlayer);
    }

    public float GetCurrentSpeed()
    {
        float healthPercentage = base.enemyStats.currentHealth / base.enemyStats.maxHealth;
        float maxSpeedMultiplier = 1.5f;

        return Mathf.Lerp(base.enemyStats.speed, base.enemyStats.speed * maxSpeedMultiplier, 1 - healthPercentage);
    }
}
