using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    public int currentHealth;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => enemyData.maxHealth;

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        currentHealth = data.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth < 0) currentHealth = 0;

        if(currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > enemyData.maxHealth) currentHealth = enemyData.maxHealth;
    }

    public void Die()
    {

    }
}
