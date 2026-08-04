using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private StatusHolderUI statusHolderUI; // Tham chiếu đến StatusArea UI

    private void Awake()
    {
        if (statusHolderUI == null)
            statusHolderUI = GetComponentInChildren<StatusHolderUI>();
        if (statusHolderUI == null)
            statusHolderUI = FindAnyObjectByType<StatusHolderUI>();
    }

    // Dictionary lưu số stack hiện tại của từng loại Buff/Debuff <buffID, số_stack>
    private Dictionary<string, int> activeStatusStacks = new Dictionary<string, int>();

    // Dictionary lưu thông tin BuffData tương ứng với buffID
    private Dictionary<string, BuffData> buffDataMap = new Dictionary<string, BuffData>();

    /// <summary>
    /// Hàm thêm hoặc thay đổi số lượng stack của Buff/Debuff
    /// </summary>
    public void ApplyStatus(BuffData buffData, int amount)
    {
        if (buffData == null) return;

        string id = buffData.BuffID;

        // Lưu dữ liệu BuffData nếu chưa có
        if (!buffDataMap.ContainsKey(id))
        {
            buffDataMap.Add(id, buffData);
        }

        // Tính tổng số stack mới
        int currentStack = activeStatusStacks.ContainsKey(id) ? activeStatusStacks[id] : 0;
        int newStack = currentStack + amount;

        if (newStack > 0)
        {
            activeStatusStacks[id] = newStack;
            // Cập nhật lên UI
            if (statusHolderUI != null) statusHolderUI.SetStatus(id, buffData.BuffName, buffData.BuffIcon, newStack);
        }
        else
        {
            // Nếu stack <= 0 thì xóa khỏi danh sách và xóa UI
            RemoveStatus(id);
        }
    }

    /// <summary>
    /// Set số stack chính xác (dùng khi đồng bộ từ nguồn khác, không cộng dồn)
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
    /// Xóa hẳn 1 Buff/Debuff
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
    /// Trả về số stack hiện tại của 1 Buff/Debuff
    /// </summary>
    public int GetStatusStack(string buffID)
    {
        return activeStatusStacks.TryGetValue(buffID, out int stack) ? stack : 0;
    }

    /// <summary>
    /// Hàm gọi khi kết thúc lượt (Trigger hiệu ứng như mất máu do Độc/Cháy)
    /// </summary>
    public void OnTurnEnd()
    {
        // Ví dụ: Xử lý Độc (Gây sát thương = số stack Độc, sau đó giảm 1 stack)
        if (activeStatusStacks.ContainsKey("Poison"))
        {
            int poisonDamage = activeStatusStacks["Poison"];
            Debug.Log($"{gameObject.name} is poisoned and takes {poisonDamage} damage!");

            // Giảm 1 stack độc sau mỗi lượt
            ApplyStatus(buffDataMap["Poison"], -1);
        }

        // Ví dụ: Xử lý Cháy
        if (activeStatusStacks.ContainsKey("Burn"))
        {
            int burnDamage = activeStatusStacks["Burn"];
            Debug.Log($"{gameObject.name} is burning and takes {burnDamage} damage!");

            // Giảm 1 stack cháy
            ApplyStatus(buffDataMap["Burn"], -1);
        }
    }
}
