using UnityEngine;
using TMPro;

public class EnemyUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshPro enemyHealthText;

    private EnemyStats enemy;

    void Awake()
    {
        
        enemy = GetComponentInParent<EnemyStats>();
    }

    void OnEnable()
    {
        if (enemy != null)
        {
            enemy.HealthChanged += UpdateHealth;
        }
    }

    void OnDisable()
    {
        if (enemy != null)
        {
            enemy.HealthChanged -= UpdateHealth;
        }
    }

    void UpdateHealth(int currentHealth, int maxHealth)
    {
        enemyHealthText.text = $"{currentHealth}/{maxHealth}";
    }
}
