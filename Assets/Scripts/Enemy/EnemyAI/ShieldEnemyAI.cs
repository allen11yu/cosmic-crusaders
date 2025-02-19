using UnityEngine;

public class ShieldEnemyAI : EnemyAIBase
{
    [Header("Shield Following Settings")]
    public float followDistance = 10f;
    private GameObject currentTarget = null;
    private GameObject shieldGenAI;
    private bool shieldsDeactivated = false;

    protected override void Start()
    {
        base.Start();
        FindClosestTarget();

        shieldGenAI = GameObject.FindWithTag("ShieldGenAI"); 
        if (shieldGenAI == null)
        {
            Debug.LogWarning("ShieldGenAI not found in the scene.");
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (currentTarget != null)
        {
            if (currentTarget == null)
            {
                Debug.Log($"ShieldEnemyAI: Current target {currentTarget.name} no longer exists.");
                FindClosestTarget();
            }
            else
            {
                MoveTowardsTarget();
            }
        }
    }

    public void Update()
    {
        ActivateShield();
    }

    public void ActivateShield()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    
        foreach (GameObject enemy in enemies)
        {

            Transform shield = enemy.transform.Find("Shield");
        
            if (shield != null && !shield.gameObject.activeSelf)
            {
                shield.gameObject.SetActive(true); 
            }
        }
    }

private void DisableShieldsOnEnemies()
{

    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    
    foreach (GameObject enemy in enemies)
    {

        Transform shield = enemy.transform.Find("Shield");
        
        if (shield != null && shield.gameObject.activeSelf)
        {
            shield.gameObject.SetActive(false); // Deactivate the shield
            Debug.Log($"Shield for enemy {enemy.name} deactivated.");
        }
    }
}


    private void FindClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                if (enemy == this.gameObject)
                    continue;

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                currentTarget = nearestEnemy;
                Debug.Log($"ShieldEnemyAI: New target set to {currentTarget.name} at distance {shortestDistance}");
            }
        }
        else
        {
            currentTarget = null;
            Debug.LogWarning("ShieldEnemyAI: No available targets found.");
        }
    }

    private void MoveTowardsTarget()
    {
        if (currentTarget == null)
        {
            FindClosestTarget();
            return;
        }

        Vector3 targetPosition = currentTarget.transform.position;
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget > followDistance)
        {
            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
            HandleMovement(activeForwardSpeed);

            Vector3 directionToEnemy = directionToTarget;
            HandleRotation(directionToEnemy);
        }
        else if (distanceToTarget < followDistance * 0.8f)
        {
            Vector3 directionAway = (transform.position - targetPosition).normalized;
            activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, enemyStats.speed * 0.75f, enemyStats.forwardAcceleration * Time.fixedDeltaTime);
            HandleMovement(activeForwardSpeed);
            HandleRotation(directionAway);
        }
    }
}



