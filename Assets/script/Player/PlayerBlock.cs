using UnityEngine;
using System;

public class PlayerBlock : MonoBehaviour
{
    private int currentBlock;
    public int CurrentBlock => currentBlock;
    public event Action<int> OnBlockChanged;

    [SerializeField] private GameObject blockPopupPrefab;

    public void AddBlock(int amount)
    {
        currentBlock += amount;
        OnBlockChanged?.Invoke(currentBlock);
        ShowBlockPopup(amount);
    }

    private void ShowBlockPopup(int amount)
    {
        if (blockPopupPrefab == null) return;
        GameObject popup = Instantiate(blockPopupPrefab, transform);
        popup.transform.localPosition = Vector3.zero;
        popup.GetComponent<BlockPopup>()?.Play(amount);
    }

    public void ResetBlock()
    {
        currentBlock = 0;
        OnBlockChanged?.Invoke(currentBlock);
    }

    public int AbsorbDamage(int damage)
    {
        if(currentBlock >= damage)
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
