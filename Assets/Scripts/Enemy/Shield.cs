using UnityEngine;

public class Shield : MonoBehaviour
{
    public int shieldHealth = 5;  
    private Animator animator;
    private Renderer shieldRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        shieldRenderer = GetComponent<Renderer>();
        shieldRenderer.enabled = false;  
    }

    public void ActivateShield()
    {
        if (!shieldRenderer.enabled)
        {
            shieldRenderer.enabled = true;  
            // animator.SetTrigger("shield");  
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            if (!shieldRenderer.enabled)
            {
                ActivateShield();
            }

            shieldHealth--;
            PlayerBulletPool.Instance.ReturnBullet(other.gameObject);

            if (shieldHealth <= 0)
            {
                Destroy(gameObject);  
            }
        }
    }
}


