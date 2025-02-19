using UnityEngine;

public class WingAnimation : MonoBehaviour
{
    private Animator animator; 
    private bool isMoving = false;  
    public GameObject leftJets; 
    public GameObject rightJets; 
    private ParticleSystem[] leftParticleSystems;  
    private ParticleSystem[] rightParticleSystems; 

    void Start()
    {
    animator = GetComponent<Animator>();

        if (leftJets != null)
        {
            leftParticleSystems = leftJets.GetComponentsInChildren<ParticleSystem>();  
            foreach (var ps in leftParticleSystems)
            {
                ps.Stop(); 
            }
        }

        if (rightJets != null)
            {
            rightParticleSystems = rightJets.GetComponentsInChildren<ParticleSystem>();  
            foreach (var ps in rightParticleSystems)
            {
                ps.Stop();  
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isMoving = true;
            animator.SetBool("isMoving", isMoving); 

            if (leftParticleSystems != null)
            {
                foreach (ParticleSystem ps in leftParticleSystems)
                {
                    ps.Play();  
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            isMoving = false;
            animator.SetBool("isMoving", isMoving);  
            
            if (leftParticleSystems != null)
            {
                foreach (ParticleSystem ps in leftParticleSystems)
                {
                    ps.Stop();  
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            isMoving = true;
            animator.SetBool("isMoving", isMoving);  

            if (rightParticleSystems != null)
            {
                foreach (ParticleSystem ps in rightParticleSystems)
                {
                    ps.Play();  
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            isMoving = false;
            animator.SetBool("isMoving", isMoving);  
            
            if (rightParticleSystems != null)
            {
                foreach (ParticleSystem ps in rightParticleSystems)
                {
                    ps.Stop();  
                }
            }
        }
    }
}


