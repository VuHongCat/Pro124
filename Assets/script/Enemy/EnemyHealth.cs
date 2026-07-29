using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Components")]
    [SerializeField] private EnemyBlock enemyBlock;

    private int currentHealth;
    private int maxHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // Cho EnemyTargetManager lấy EnemyData
    public EnemyData EnemyData => enemyData;

    public event Action<int, int> OnHealthChanged;
    public event Action<EnemyHealth> OnEnemyDeath;

    private bool isDead;

    private void Awake()
    {
        // Tự tìm EnemyBlock nếu chưa kéo vào Inspector
        if (enemyBlock == null)
        {
            enemyBlock = GetComponent<EnemyBlock>();
        }
    }

    public void Initialize(EnemyData data)
    {
        if (data == null)
        {
            Debug.LogError(
                "EnemyHealth: EnemyData is NULL on " + gameObject.name
            );
            return;
        }

        enemyData = data;

        maxHealth = data.maxHealth;
        currentHealth = maxHealth;

        isDead = false;

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    public void TakeDamage(int damage)
    {
        // Enemy đã chết thì không nhận damage nữa
        if (isDead)
            return;

        // Không cho damage âm
        if (damage < 0)
            damage = 0;

        // Block hấp thụ damage trước
        if (enemyBlock != null)
        {
            damage = enemyBlock.AbsorbDamage(damage);
        }

        // Nếu Block hấp thụ toàn bộ damage
        if (damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        Debug.Log(
            enemyData.enemyName +
            " takes " +
            damage +
            " damage. HP: " +
            currentHealth +
            "/" +
            maxHealth
        );

        // Enemy chết
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

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );
    }

    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        currentHealth = 0;

        OnHealthChanged?.Invoke(
            currentHealth,
            maxHealth
        );

        Debug.Log(
            enemyData.enemyName + " died!"
        );

        // Báo cho BattleManager
        OnEnemyDeath?.Invoke(this);

        // Xóa enemy khỏi Battle
        Destroy(gameObject);
    }
}