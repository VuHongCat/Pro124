using UnityEngine;
using System.Collections.Generic;

public class PlayerStatus : MonoBehaviour
{
    private readonly Dictionary<StatusType, int> statuses = new();

    public void AddStatus(StatusType type, int amount)
    {
        if (!statuses.ContainsKey(type)) statuses[type] = 0;

        statuses[type] += amount;
    }

    public int GetStatus(StatusType type)
    {
        if(statuses.TryGetValue(type, out int value))
        {
            return value;
        }
        return 0;
    }

    public void RemoveStatus(StatusType type)
    {
        statuses.Remove(type);
    }
}
