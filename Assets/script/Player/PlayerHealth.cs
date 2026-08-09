using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerBlock playerBlock;
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private GameObject healPopupPrefab;
    [SerializeField] private GameObject damageNumberPrefab;

    private int currentHealth;
    private int maxHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDeath;
    public event Action<int> OnDamageTaken;

    private void Awake()
    {
        Initialize();
        if (playerBlock == null) playerBlock = GetComponent<PlayerBlock>();
        if (playerStatus == null) playerStatus = GetComponent<PlayerStatus>();
    }

    public void Initialize()
    {
        if (RunSession.RunActive && RunSession.PlayerMaxHealth > 0)
        {
            maxHealth = RunSession.PlayerMaxHealth;
            currentHealth = RunSession.PlayerCurrentHealth;
        }
        else
        {
            maxHealth = playerData.maxHealth;
            currentHealth = maxHealth;
            RunSession.StartNewRun();
            RunSession.PlayerMaxHealth = maxHealth;
            RunSession.PlayerCurrentHealth = currentHealth;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage, bool reflectable = true)
    {
        if (playerStatus != null && playerStatus.GetStatus(StatusType.Immortal) > 0)
            return;
        if (playerStatus != null && playerStatus.GetStatus(StatusType.Vulnerable) > 0)
            damage = Mathf.RoundToInt(damage * 1.5f);

        ScreenShake.Instance?.Shake();

        if (playerBlock != null)
        {
            damage = playerBlock.AbsorbDamage(damage);
        }
        if (damage <= 0) return;
        currentHealth -= damage;
        if(currentHealth < 0) currentHealth = 0;
        RunSession.PlayerCurrentHealth = currentHealth;
        GetComponent<EnemyHitVFX>()?.Play();
        GetComponent<PlayerDisplay>()?.Punch();
        ShowDamageNumber(damage);

        if (reflectable)
            OnDamageTaken?.Invoke(damage);
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
        RunSession.PlayerCurrentHealth = currentHealth;

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        ShowHealPopup(amount);
    }

    private void ShowHealPopup(int amount)
    {
        if (healPopupPrefab == null) return;
        GameObject popup = Instantiate(healPopupPrefab, transform);
        popup.transform.localPosition = Vector3.zero;
        popup.GetComponent<BlockPopup>()?.Play(amount);
    }

    private void ShowDamageNumber(int amount)
    {
        if (damageNumberPrefab == null) return;
        GameObject number = Instantiate(damageNumberPrefab, transform);
        RectTransform rect = GetComponent<RectTransform>();
        float yOffset = rect != null ? rect.rect.height * 0.5f + 10f : 120f;
        number.transform.localPosition = new Vector3(0f, yOffset, 0f);
        number.GetComponent<DamageNumber>().Play(amount);
    }

    private void Die()
    {
        Debug.Log("Player Die");
        OnPlayerDeath?.Invoke();
    }
}
