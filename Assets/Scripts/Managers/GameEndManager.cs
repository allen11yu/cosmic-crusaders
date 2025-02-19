using UnityEngine;
using TMPro;
using UnityEngine.UI;  
using System.Collections;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("Game End UI")]
    public GameObject gameEndGameObject;
    public TextMeshProUGUI gameEndText;

    public RawImage winRawImage;  
    public RawImage loseRawImage; 


    public float winDelay = 5f;
    public float loseDelay = 5f; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (gameEndGameObject != null)
        {
            gameEndGameObject.SetActive(false);
        }
    }

    private void Start()
    {
    }

    void Update()
    {
        if (FindObjectOfType<EnemySpawner>().currentWave >= FindObjectOfType<EnemySpawner>().enemiesPerWave.Length && !gameEndGameObject.activeSelf)
        {
            TriggerGameEnd(true); 
        }
    }

    /*public void TriggerGameEnd(bool isWin)
    {
        PauseMenuToggle.canPause = false;
        Cursor.visible = true;
        gameEndGameObject.SetActive(true);
        gameEndText.text = isWin ? "You Win!" : "You Lose!";


        if (isWin)
        {
            StartCoroutine(ShowWinImageWithDelay());  
            loseRawImage.gameObject.SetActive(false); 
        }
        else
        {
            winRawImage.gameObject.SetActive(false);  
            loseRawImage.gameObject.SetActive(true);  
        }

        Time.timeScale = 0;
    }

    private IEnumerator ShowWinImageWithDelay()
    {

        yield return new WaitForSeconds(winDelay);
        winRawImage.gameObject.SetActive(true);
    }*/

    public void TriggerGameEnd(bool isWin)
    {
        StartCoroutine(TriggerGameEndWithDelay(isWin));
    }

    private IEnumerator TriggerGameEndWithDelay(bool isWin)
    {   
        yield return new WaitForSeconds(5f);  
        Time.timeScale = 0;

    
        PauseMenuToggle.canPause = false;
        Cursor.visible = true;

    
        gameEndGameObject.SetActive(true);
        gameEndText.text = isWin ? "You Win!" : "You Lose!";


        if (isWin)
        {
            StartCoroutine(ShowWinImageWithDelay());  
            loseRawImage.gameObject.SetActive(false); 
        }
        else
        {
            StartCoroutine(ShowLoseImageWithDelay());
            winRawImage.gameObject.SetActive(false);  
            loseRawImage.gameObject.SetActive(true);  
        }
    }

    private IEnumerator ShowWinImageWithDelay()
    {

        yield return new WaitForSeconds(2f);  
        winRawImage.gameObject.SetActive(true);
    }

    private IEnumerator ShowLoseImageWithDelay()
    {

        yield return new WaitForSeconds(2f);  
        loseRawImage.gameObject.SetActive(true);
    }
}








