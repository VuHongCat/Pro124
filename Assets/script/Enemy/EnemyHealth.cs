using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBlock enemyBlock;
    [SerializeField] private EnemyStatus enemyStatus;
    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private GameObject healPopupPrefab;

    public int currentHealth;
    public int maxHealth;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsBoss => enemyData != null && enemyData.isBoss;

    public event Action<int, int> OnHealthChanged;
    public event Action<EnemyHealth> OnEnemyDeath;
    public event Action<int> OnDamaged;

    private void Awake()
    {
        if (enemyBlock == null) enemyBlock = GetComponent<EnemyBlock>();
        if (enemyStatus == null) enemyStatus = GetComponent<EnemyStatus>();
    }

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, bool counterable = true)
    {
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Immortal) > 0)
            return;
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Vulnerable) > 0)
            damage = Mathf.RoundToInt(damage * 1.5f);
        if (enemyBlock != null)
            damage = enemyBlock.AbsorbDamage(damage);
        if (damage <= 0)
            return;
        ScreenShake.Instance?.Shake();
        ShowDamageNumber(damage);
        GetComponent<EnemyHitVFX>()?.Play();
        GetComponent<EnemyDisplay>()?.Punch();
        if (counterable)
            OnDamaged?.Invoke(damage);
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
        ShowHealPopup(amount);
    }

    private void ShowHealPopup(int amount)
    {
        if (healPopupPrefab == null)
            return;

        GameObject popup = Instantiate(healPopupPrefab, transform);
        popup.transform.localPosition = Vector3.zero;
        popup.GetComponent<BlockPopup>()?.Play(amount);
    }

    private void ShowDamageNumber(int amount)
    {
        if (damageNumberPrefab == null)
            return;

        GameObject number = Instantiate(damageNumberPrefab, transform);
        RectTransform enemyRect = GetComponent<RectTransform>();
        float yOffset = enemyRect != null ? enemyRect.rect.height * 0.5f + 10f : 120f;
        number.transform.localPosition = new Vector3(0f, yOffset, 0f);
        number.GetComponent<DamageNumber>().Play(amount);
    }

    public void Die()
    {
        OnEnemyDeath?.Invoke(this);
    }
}

