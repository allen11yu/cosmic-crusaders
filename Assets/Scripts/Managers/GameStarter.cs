using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Play Game button clicked!");
        SceneManager.LoadScene("CosmicCrusader");
        Cursor.lockState = CursorLockMode.Confined;
        PauseMenuToggle.canPause = true;
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
