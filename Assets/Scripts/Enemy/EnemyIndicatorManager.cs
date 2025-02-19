using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyIndicatorManager : MonoBehaviour
{
    public Transform player;               // Reference to the player
    public Camera mainCamera;              // Reference to the main camera
    public GameObject indicatorPrefab;     // Prefab for the indicator
    public float detectionDistance = 100f; // Maximum detection distance

    private RectTransform canvasRect;      // RectTransform of the Canvas
    private List<GameObject> trackedEnemies; // List to track all spawned enemies

    void Awake()
    {
        trackedEnemies = new List<GameObject>(); // Initialization in Awake
    }

    void Start()
    {
        // Step 1: Find the Canvas RectTransform
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Canvas not found in the scene!");
        }

        // Step 2: Find all enemies with tag "Enemy" or "Final Boss"
        trackedEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        trackedEnemies.AddRange(GameObject.FindGameObjectsWithTag("FinalBoss"));
    }

    void Update()
    {
        // Step 3: Update indicators for all tracked enemies
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemy = trackedEnemies[i];

            if (enemy == null)
            {
                // Step 4: Remove destroyed enemies from the list
                trackedEnemies.RemoveAt(i);
                continue;
            }

            float distanceToEnemy = Vector3.Distance(player.position, enemy.transform.position);

            if (distanceToEnemy <= detectionDistance)
            {
                Vector3 screenPosition = mainCamera.WorldToViewportPoint(enemy.transform.position);

                // Step 5: Check if the enemy is off-screen
                if (screenPosition.x < 0 || screenPosition.x > 1 || screenPosition.y < 0 || screenPosition.y > 1 || screenPosition.z < 0)
                {
                    // Step 6: Enemy is off-screen, show the indicator at the edge of the screen
                    Vector2 indicatorPosition = GetScreenEdgePosition(screenPosition);
                    GameObject indicator = GetOrCreateIndicator(enemy);
                    PositionIndicator(indicator, indicatorPosition, enemy.transform.position);
                }
                else
                {
                    // Step 7: Enemy is on-screen, hide the indicator
                    HideIndicator(enemy);
                }
            }
            else
            {
                // Step 8: Enemy is out of range, hide the indicator
                HideIndicator(enemy);
            }
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (trackedEnemies == null)
        {
            trackedEnemies = new List<GameObject>();
        }
        // Step 9: Register a new enemy if not already tracked
        if (!trackedEnemies.Contains(enemy))
        {
            trackedEnemies.Add(enemy);
        }
    }

    private Vector2 GetScreenEdgePosition(Vector3 screenPosition)
    {
        // Step 10: Calculate screen edge position for off-screen indicators
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 screenDirection = new Vector2(screenPosition.x - 0.5f, screenPosition.y - 0.5f).normalized;

        float xFactor = Screen.width / 2;
        float yFactor = Screen.height / 2;
        float m = screenDirection.y / screenDirection.x;

        Vector2 edgePosition = screenCenter;

        if (Mathf.Abs(m) > yFactor / xFactor)
        {
            // Indicator is closer to the top/bottom edge
            edgePosition.y = screenDirection.y > 0 ? Screen.height : 0;
            edgePosition.x = screenCenter.x + (edgePosition.y - screenCenter.y) / m;
        }
        else
        {
            // Indicator is closer to the left/right edge
            edgePosition.x = screenDirection.x > 0 ? Screen.width : 0;
            edgePosition.y = screenCenter.y + (edgePosition.x - screenCenter.x) * m;
        }

        return edgePosition;
    }

    private Vector2 GetScreenEdgePositionXXX(Vector3 screenPosition)
    {
        // Step 10: Calculate screen edge position for off-screen indicators
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 screenDirection = new Vector2(screenPosition.x - 0.5f, screenPosition.y - 0.5f).normalized;

        float edgeDistance = Mathf.Min(Screen.width, Screen.height) / 2 - 50; // Offset to avoid overlap
        return screenCenter + screenDirection * edgeDistance;
    }

    private GameObject GetOrCreateIndicator(GameObject enemy)
    {
        // Step 11: Get or create an indicator for the enemy
        GameObject indicator = enemy.GetComponent<IndicatorReference>()?.indicator;

        if (indicator == null)
        {
            indicator = Instantiate(indicatorPrefab, canvasRect);
            IndicatorReference reference = enemy.AddComponent<IndicatorReference>();
            reference.indicator = indicator;
        }

        indicator.SetActive(true); // Ensure the indicator is visible
        return indicator;
    }

    private void PositionIndicator(GameObject indicator, Vector3 position, Vector3 enemyPosition)
    {
        // Step 12: Position the indicator on screen
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();
        rectTransform.position = position;

        // Step 13: Rotate the indicator to point toward the enemy
        Vector3 direction = enemyPosition - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void HideIndicator(GameObject enemy)
    {
        // Step 14: Hide the indicator when the enemy is out of range or on-screen
        IndicatorReference reference = enemy.GetComponent<IndicatorReference>();
        if (reference != null && reference.indicator != null)
        {
            reference.indicator.SetActive(false);
        }
    }
}

public class IndicatorReference : MonoBehaviour
{
    public GameObject indicator; // Reference to the indicator associated with this enemy
}
