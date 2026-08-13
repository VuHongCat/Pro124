using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private StatusHolderUI statusHolderUI; // Reference to the StatusArea UI

    private void Awake()
    {
        if (statusHolderUI == null)
            statusHolderUI = GetComponentInChildren<StatusHolderUI>();
        if (statusHolderUI == null)
            statusHolderUI = FindAnyObjectByType<StatusHolderUI>();
    }

    // Dictionary storing the current stack count of each Buff/Debuff <buffID, stack_count>
    private Dictionary<string, int> activeStatusStacks = new Dictionary<string, int>();

    // Dictionary storing the BuffData corresponding to each buffID
    private Dictionary<string, BuffData> buffDataMap = new Dictionary<string, BuffData>();

    /// <summary>
    /// Adds or changes the stack count of a Buff/Debuff
    /// </summary>
    public void ApplyStatus(BuffData buffData, int amount)
    {
        if (buffData == null) return;

        string id = buffData.BuffID;

        // Store BuffData if it doesn't exist yet
        if (!buffDataMap.ContainsKey(id))
        {
            buffDataMap.Add(id, buffData);
        }

        // Calculate the new total stack count
        int currentStack = activeStatusStacks.ContainsKey(id) ? activeStatusStacks[id] : 0;
        int newStack = currentStack + amount;

        if (newStack > 0)
        {
            activeStatusStacks[id] = newStack;
            // Update UI
            if (statusHolderUI != null) statusHolderUI.SetStatus(id, buffData.BuffName, buffData.BuffIcon, newStack);
        }
        else
        {
            // If stack <= 0, remove it from the list and from the UI
            RemoveStatus(id);
        }
    }

    /// <summary>
    /// Sets the exact stack count (used when syncing from another source, no accumulation)
    /// </summary>
    public void SetStatus(BuffData buffData, int stack)
    {
        if (buffData == null) return;

        string id = buffData.BuffID;

        if (!buffDataMap.ContainsKey(id))
        {
            buffDataMap.Add(id, buffData);
        }

        if (stack > 0)
        {
            activeStatusStacks[id] = stack;
            if (statusHolderUI != null) statusHolderUI.SetStatus(id, buffData.BuffName, buffData.BuffIcon, stack);
        }
        else
        {
            RemoveStatus(id);
        }
    }

    /// <summary>
    /// Removes a Buff/Debuff entirely
    /// </summary>
    public void RemoveStatus(string buffID)
    {
        if (activeStatusStacks.ContainsKey(buffID))
        {
            activeStatusStacks.Remove(buffID);
            if (statusHolderUI != null) statusHolderUI.RemoveStatus(buffID);
        }
    }

    /// <summary>
    /// Returns the current stack count of a Buff/Debuff
    /// </summary>
    public int GetStatusStack(string buffID)
    {
        return activeStatusStacks.TryGetValue(buffID, out int stack) ? stack : 0;
    }

    /// <summary>
    /// Called when the turn ends (triggers effects like damage from Poison/Burn)
    /// </summary>
    public void OnTurnEnd()
    {
        // Example: Handle Poison (deals damage = Poison stack, then reduces 1 stack)
        if (activeStatusStacks.ContainsKey("Poison"))
        {
            int poisonDamage = activeStatusStacks["Poison"];
            Debug.Log($"{gameObject.name} is poisoned and takes {poisonDamage} damage!");

            // Reduce poison stack by 1 each turn
            ApplyStatus(buffDataMap["Poison"], -1);
        }

        // Example: Handle Burn
        if (activeStatusStacks.ContainsKey("Burn"))
        {
            int burnDamage = activeStatusStacks["Burn"];
            Debug.Log($"{gameObject.name} is burning and takes {burnDamage} damage!");

            // Reduce burn stack by 1
            ApplyStatus(buffDataMap["Burn"], -1);
        }
    }
}
