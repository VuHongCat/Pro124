using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBlock enemyBlock;

    public int currentHealth;
    public int maxHealth;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action<EnemyHealth> OnEnemyDeath;

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (enemyBlock != null)
        {
            damage = enemyBlock.AbsorbDamage(damage);
        }
        if (damage <= 0)
            return;
        currentHealth -= damage;
        if(currentHealth < 0) currentHealth = 0;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > enemyData.maxHealth) currentHealth = enemyData.maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Die()
    {
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
