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
        AudioManager.PlayShieldGain();
    }

    private void ShowBlockPopup(int amount)
    {
        ShowBlockPopup(amount, false);
    }

    private void ShowBlockPopup(int amount, bool stayCenter)
    {
        if (blockPopupPrefab == null) return;
        GameObject popup = Instantiate(blockPopupPrefab, transform);
        popup.transform.localPosition = Vector3.zero;
        popup.GetComponent<BlockPopup>()?.Play(amount, stayCenter);
    }

    public void ResetBlock()
    {
        currentBlock = 0;
        OnBlockChanged?.Invoke(currentBlock);
    }

    public int AbsorbDamage(int damage)
    {
        int absorbed = Mathf.Min(currentBlock, damage);
        if (absorbed > 0)
        {
            currentBlock -= absorbed;
            OnBlockChanged?.Invoke(currentBlock);
            AudioManager.PlayShieldTakeDamage();

            if (absorbed == damage)
                ShowBlockPopup(currentBlock, true);
        }

        return damage - absorbed;
    }
}
