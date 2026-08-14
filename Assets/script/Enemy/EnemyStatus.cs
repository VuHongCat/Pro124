using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private class StatusEntry
    {
        public int Stacks;
        public int Turns;
    }

    private readonly Dictionary<StatusType, StatusEntry> statuses = new();
    public event System.Action OnStatusChanged;

    [Header("UI Sync")]
    [SerializeField] private StatusHolderUI statusUI;
    [SerializeField] private GameObject statusPopupPrefab;
    [SerializeField] private BuffData strengthData;
    [SerializeField] private BuffData weakData;
    [SerializeField] private BuffData vulnerableData;
    [SerializeField] private BuffData stunData;
    [SerializeField] private BuffData counterData;
    [SerializeField] private BuffData immortalData;
    [SerializeField] private BuffData bleedData;
    [SerializeField] private BuffData regenData;
    [SerializeField] private BuffData lifestealData;
    [SerializeField] private BuffData poisonData;

    private bool warnedMissingUI;
    private readonly List<StatusType> missingDataTypes = new();

    private void Awake()
    {
        if (statusUI == null)
            statusUI = GetComponentInChildren<StatusHolderUI>();
        if (statusUI == null)
            statusUI = FindAnyObjectByType<StatusHolderUI>();
        if (statusPopupPrefab == null)
            statusPopupPrefab = Resources.Load<GameObject>("StatusPopup");
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
                Debug.LogWarning($"{name}: EnemyStatus could not find StatusHolderUI (StatusArea)!", this);
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
        SyncOne(StatusType.Poison, poisonData);
    }

    private void SyncOne(StatusType type, BuffData data)
    {
        if (data == null)
        {
            if (!missingDataTypes.Contains(type))
            {
                missingDataTypes.Add(type);
                Debug.LogWarning($"{name}: no BuffData assigned for {type} in EnemyStatus!", this);
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
        if (amount < 0)
        {
            if (statuses.TryGetValue(type, out StatusEntry consume))
            {
                consume.Stacks += amount;
                if (consume.Stacks <= 0)
                    statuses.Remove(type);
            }
            OnStatusChanged?.Invoke();
            return;
        }

        if (!statuses.TryGetValue(type, out StatusEntry entry))
        {
            statuses[type] = new StatusEntry
            {
                Stacks = amount,
                Turns = duration
            };
        }
        else
        {
            entry.Stacks += amount;
            entry.Turns = Mathf.Max(entry.Turns, duration);
        }

        if (amount > 0)
            SpawnStatusPopup(type, amount);

        OnStatusChanged?.Invoke();
    }

    private BuffData GetBuffData(StatusType type)
    {
        switch (type)
        {
            case StatusType.Strength:     return strengthData;
            case StatusType.Weak:         return weakData;
            case StatusType.Vulnerable:   return vulnerableData;
            case StatusType.Stun:         return stunData;
            case StatusType.Counter:      return counterData;
            case StatusType.Immortal:     return immortalData;
            case StatusType.Bleed:        return bleedData;
            case StatusType.Regen:        return regenData;
            case StatusType.Lifesteal:    return lifestealData;
            case StatusType.Poison:       return poisonData;
            default:                      return null;
        }
    }

    private void SpawnStatusPopup(StatusType type, int amount)
    {
        if (statusPopupPrefab == null) return;
        BuffData data = GetBuffData(type);
        if (data == null || data.BuffIcon == null) return;

        GameObject go = Instantiate(statusPopupPrefab, transform);
        go.transform.localPosition = Vector3.zero;
        StatusPopup popup = go.GetComponent<StatusPopup>();
        if (popup != null)
            popup.Play(data.Type, amount);
    }

    public int GetStatus(StatusType type)
    {
        if (statuses.TryGetValue(type, out StatusEntry entry))
            return entry.Stacks;
        return 0;
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
            statuses[key].Turns--;
            if (statuses[key].Turns <= 0)
                expired.Add(key);
        }
        foreach (var key in expired)
            statuses.Remove(key);
        OnStatusChanged?.Invoke();
    }
}
