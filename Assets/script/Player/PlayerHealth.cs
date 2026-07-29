using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private bool isDead = false;

    public event Action OnPlayerDeath;
    public event Action<int, int> OnHealthChanged;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private const string CurrentHealthKey = "Run_CurrentHealth";
    private const string MaxHealthKey = "Run_MaxHealth";

    private void Start()
    {
        LoadHealth();

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        Debug.Log(
            "[PlayerHealth] LOAD HP = " +
            currentHealth + "/" + maxHealth
        );
    }

    private void LoadHealth()
    {
        if (PlayerPrefs.HasKey(CurrentHealthKey))
        {
            currentHealth = PlayerPrefs.GetInt(
                CurrentHealthKey
            );

            maxHealth = PlayerPrefs.GetInt(
                MaxHealthKey,
                maxHealth
            );
        }
        else
        {
            currentHealth = maxHealth;
            SaveHealth();
        }

        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        SaveHealth();

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        Debug.Log(
            "[PlayerHealth] HP = " +
            currentHealth + "/" + maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        if (amount <= 0)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        SaveHealth();

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    private void SaveHealth()
    {
        PlayerPrefs.SetInt(
            CurrentHealthKey,
            currentHealth
        );

        PlayerPrefs.SetInt(
            MaxHealthKey,
            maxHealth
        );

        PlayerPrefs.Save();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "[PlayerHealth] PLAYER DEAD"
        );

        Debug.Log(
            "================================"
        );

        // Gọi GameOverManager
        OnPlayerDeath?.Invoke();
    }

    public static void ResetRunHealth()
    {
        PlayerPrefs.SetInt(
            CurrentHealthKey,
            100
        );

        PlayerPrefs.SetInt(
            MaxHealthKey,
            100
        );

        PlayerPrefs.Save();

        Debug.Log(
            "[PlayerHealth] RESET HP = 100/100"
        );
    }
}