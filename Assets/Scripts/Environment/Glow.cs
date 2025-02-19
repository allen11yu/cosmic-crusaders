using UnityEngine;

public class Glow : MonoBehaviour
{
    private Animator asteroidFade;  
    private bool isPlayerNear = false;  

    void Start()
    {
        asteroidFade = GetComponent<Animator>();
    }

    void Update()
    {
        if (isPlayerNear && !asteroidFade.GetCurrentAnimatorStateInfo(0).IsName("Fade"))
        {
            asteroidFade.SetTrigger("PlayerNear");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
