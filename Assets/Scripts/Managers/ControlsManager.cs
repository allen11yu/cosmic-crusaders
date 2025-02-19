using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    public GameObject controlsPanel;

    public void ToggleControls()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
        Cursor.visible = true;
        Time.timeScale = 0f;
        PauseMenuToggle.canPause = false;
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 0f;
        PauseMenuToggle.canPause = true;
    }

    void Update()
    {
        if (controlsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseControls();
        }
    }
}
