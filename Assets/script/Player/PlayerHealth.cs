using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerBlock playerBlock;

    private int currentHealth;
    private int maxHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDeath;

    private void Awake()
    {
        Initialize();
        playerBlock = GetComponent<PlayerBlock>();
    }

    public void Initialize()
    {
        maxHealth = playerData.maxHealth;
        currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (playerBlock != null)
        {
            damage = playerBlock.AbsorbDamage(damage);
        }
        if (damage <= 0) return;
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
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player Die");
        OnPlayerDeath?.Invoke();
    }
}
