using UnityEngine;
using TMPro;

public class PlayerShield : MonoBehaviour
{
    public float shieldHealth = 100f;
    public int maxShieldHealth = 100;
    private Renderer shieldRenderer;
    private Collider shieldCollider;
    private Animator animator;
    private GameObject shieldObject;
    public TextMeshProUGUI shieldHealthText;
    public PlayerStats playerStats;

    private bool shieldActive = false;
    public float shieldDecayRate = 2f;
    private float timeSinceLastDecay = 0f;

    public float shieldRegenerationRate = 1f;
    private float timeSinceLastRegeneration = 0f;

    void Start()
    {
        shieldRenderer = GetComponent<Renderer>();
        shieldCollider = GetComponent<Collider>();
        animator = GetComponentInParent<Animator>();
        shieldObject = gameObject;

        shieldRenderer.enabled = false;
        shieldCollider.enabled = false;

        shieldHealthText.gameObject.SetActive(false);
        UpdateShieldHealthUI();
    }

    void Update()
    {
        if (playerStats.isShieldUnlock && Input.GetKeyDown(KeyCode.S))
        {
            ToggleShield();
            shieldHealthText.gameObject.SetActive(true);
        }

        if (shieldActive)
        {
            timeSinceLastDecay += Time.deltaTime;

            if (timeSinceLastDecay >= 1f)
            {
                shieldHealth -= shieldDecayRate;
                timeSinceLastDecay = 0f;

                if (shieldHealth < 0)
                {
                    shieldHealth = 0f;
                }

                UpdateShieldHealthUI();

                if (shieldHealth <= 0)
                {
                    DeactivateShield();
                }
            }
        }
        else
        {
            timeSinceLastRegeneration += Time.deltaTime;

            if (timeSinceLastRegeneration >= 1f)
            {
                shieldHealth += shieldRegenerationRate;
                timeSinceLastRegeneration = 0f;

                if (shieldHealth > maxShieldHealth)
                {
                    shieldHealth = maxShieldHealth;
                }

                UpdateShieldHealthUI();
            }
        }
    }

    public void ToggleShield()
    {
        if (shieldActive)
        {
            shieldRenderer.enabled = false;
            shieldCollider.enabled = false;
            shieldActive = false;
        }
        else
        {
            shieldRenderer.enabled = true;
            shieldCollider.enabled = true;
            shieldActive = true;

            if (animator != null)
            {
                animator.SetTrigger("shield");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") && shieldActive)
        {
            shieldHealth -= 10f;

            Destroy(other.gameObject);
            UpdateShieldHealthUI();

            if (shieldHealth <= 0)
            {
                DeactivateShield();
            }
        }
    }

    void UpdateShieldHealthUI()
    {
        if (shieldHealthText != null)
        {
            float percentage = (shieldHealth / maxShieldHealth) * 100f;
            shieldHealthText.text = "Shield: " + Mathf.RoundToInt(percentage) + "%";
        }
    }

    void DeactivateShield()
    {
        shieldActive = false;
        shieldRenderer.enabled = false;
        shieldCollider.enabled = false;
    }
}

