using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public PlayerStats playerStats;

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("FinalBoss"))
        {
            EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(playerStats.damageOnCollision);   
                playerStats.TakeDamage(Mathf.RoundToInt(enemy.damageOnCollision));

                Debug.Log($"Player collided with enemy. Dealt {playerStats.damage} damage and took {enemy.damageOnCollision} damage.");
            }
        }
    }
}
