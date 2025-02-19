using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform primaryCrosshair;
    public RectTransform secondaryCrosshair;
    public float maxRadius = 50f;

    [Header("References")]
    public Camera mainCamera;

    public float crosshairPadding = 10f; 

    void Start()
    {
        Cursor.visible = false;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        UpdateSecondaryCrosshairPosition();
    }

    void UpdateSecondaryCrosshairPosition()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 offset = mousePosition - screenCenter;
        Vector2 normalizedOffset = new Vector2(
            offset.x / (Screen.width / 2f),
            offset.y / (Screen.height / 2f)
        );

        Vector2 crosshairOffset = normalizedOffset * maxRadius;

        if (crosshairOffset.magnitude > maxRadius)
        {
            crosshairOffset = crosshairOffset.normalized * maxRadius;
        }

        secondaryCrosshair.anchoredPosition = crosshairOffset;
    }

    public bool IsEnemyInCrosshair(Vector3 enemyWorldPosition)
    {
        if (primaryCrosshair == null || mainCamera == null)
        {
            Debug.LogError("Primary crosshair or main camera is not assigned.");
            return false;
        }

        // Convert the enemy world position to screen space
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(enemyWorldPosition);

        // Ensure the enemy is in front of the camera
        if (screenPosition.z > 0)
        {
            Rect rectWithPadding = new Rect(
                primaryCrosshair.position.x - (primaryCrosshair.rect.width / 2 + crosshairPadding),
                primaryCrosshair.position.y - (primaryCrosshair.rect.height / 2 + crosshairPadding),
                primaryCrosshair.rect.width + 2 * crosshairPadding,
                primaryCrosshair.rect.height + 2 * crosshairPadding
            );

            bool isContained = rectWithPadding.Contains(screenPosition);

            Debug.Log(isContained
                ? "Enemy is within crosshair bounds."
                : "Enemy is not within crosshair bounds.");

            return isContained;
        }
        else
        {
            Debug.Log("Enemy is behind the camera. Screen position z-value: " + screenPosition.z);
        }

        return false;
    }
}
