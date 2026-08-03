using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private Dictionary<StatusType, int> statuses = new();
    public event System.Action OnStatusChanged;

    [Header("UI Sync")]
    [SerializeField] private StatusHolderUI statusUI;
    [SerializeField] private BuffData strengthData;
    [SerializeField] private BuffData weakData;
    [SerializeField] private BuffData vulnerableData;
    [SerializeField] private BuffData stunData;
    [SerializeField] private BuffData counterData;
    [SerializeField] private BuffData immortalData;
    [SerializeField] private BuffData bleedData;
    [SerializeField] private BuffData regenData;
    [SerializeField] private BuffData lifestealData;

    private bool warnedMissingUI;
    private readonly List<StatusType> missingDataTypes = new();

    private void Awake()
    {
        if (statusUI == null)
            statusUI = GetComponentInChildren<StatusHolderUI>();
        if (statusUI == null)
            statusUI = FindAnyObjectByType<StatusHolderUI>();
        OnStatusChanged += SyncUI;
    }

    private void Start()
    {
        SyncUI();
    }

    private void OnDestroy()
    {
        OnStatusChanged -= SyncUI;
    }

    private void SyncUI()
    {
        if (statusUI == null)
        {
            if (!warnedMissingUI)
            {
                warnedMissingUI = true;
                Debug.LogWarning($"{name}: EnemyStatus không tìm thấy StatusHolderUI (StatusArea)!", this);
            }
            return;
        }
        SyncOne(StatusType.Strength, strengthData);
        SyncOne(StatusType.Weak, weakData);
        SyncOne(StatusType.Vulnerable, vulnerableData);
        SyncOne(StatusType.Stun, stunData);
        SyncOne(StatusType.Counter, counterData);
        SyncOne(StatusType.Immortal, immortalData);
        SyncOne(StatusType.Bleed, bleedData);
        SyncOne(StatusType.Regen, regenData);
        SyncOne(StatusType.Lifesteal, lifestealData);
    }

    private void SyncOne(StatusType type, BuffData data)
    {
        if (data == null)
        {
            if (!missingDataTypes.Contains(type))
            {
                missingDataTypes.Add(type);
                Debug.LogWarning($"{name}: chưa gán BuffData cho {type} trong EnemyStatus!", this);
            }
            return;
        }
        int stack = GetStatus(type);
        if (stack > 0)
            statusUI.SetStatus(data.BuffID, data.BuffName, data.BuffIcon, stack, data.Description);
        else
            statusUI.RemoveStatus(data.BuffID);
    }

    public void AddStatus(StatusType type, int amount, int duration = 1)
    {
        if (!statuses.ContainsKey(type))
            statuses[type] = 0;
        statuses[type] += amount;
        OnStatusChanged?.Invoke();
    }

    public int GetStatus(StatusType type)
    {
        return statuses.GetValueOrDefault(type, 0);
    }

    public void RemoveStatus(StatusType type)
    {
        statuses.Remove(type);
        OnStatusChanged?.Invoke();
    }

    public void OnTurnEnd()
    {
        int regen = GetStatus(StatusType.Regen);
        if (regen > 0)
        {
            EnemyHealth hp = GetComponent<EnemyHealth>();
            hp?.Heal(regen);
        }

        int bleed = GetStatus(StatusType.Bleed);
        if (bleed > 0)
        {
            EnemyHealth hp = GetComponent<EnemyHealth>();
            hp?.TakeDamage(bleed, false);
        }

        int poison = GetStatus(StatusType.Poison);
        if (poison > 0)
        {
            EnemyHealth hp = GetComponent<EnemyHealth>();
            hp?.TakeDamage(poison, false);
        }

        List<StatusType> expired = new();
        List<StatusType> keys = new(statuses.Keys);
        foreach (var key in keys)
        {
            statuses[key]--;
            if (statuses[key] <= 0)
                expired.Add(key);
        }
        foreach (var key in expired)
            statuses.Remove(key);
        OnStatusChanged?.Invoke();
    }
}
