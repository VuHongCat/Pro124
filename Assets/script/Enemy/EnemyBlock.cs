using System;
using UnityEngine;

public class EnemyBlock : MonoBehaviour
{
    private int currentBlock;

    public int CurrentBlock => currentBlock;

    public event Action<int> OnBlockChanged;

    public void AddBlock(int amount)
    {
        currentBlock += amount;
        OnBlockChanged?.Invoke(currentBlock);
    }

    public void ResetBlock()
    {
        currentBlock = 0;
        OnBlockChanged?.Invoke(currentBlock);
    }

    public int AbsorbDamage(int damage)
    {
        if (currentBlock >= damage)
        {
            currentBlock -= damage;
            OnBlockChanged?.Invoke(currentBlock);
            return 0;
        }

        damage -= currentBlock;
        currentBlock = 0;

        OnBlockChanged?.Invoke(currentBlock);

        return damage;
    }
}