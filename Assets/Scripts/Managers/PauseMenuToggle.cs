using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuToggle : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public static bool canPause = true;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) Debug.LogError("No canvas group");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && canPause)
        {
            if (canvasGroup.interactable)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                Cursor.visible = true;
                canvasGroup.alpha = 0f;
                Time.timeScale = 1f;
            }
            else
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                Cursor.visible = false;
                canvasGroup.alpha = 1f;
                Time.timeScale = 0f;
            }
        }
    }
}
