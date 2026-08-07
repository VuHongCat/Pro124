using UnityEngine;
using UnityEngine.EventSystems;

public class BattleZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private TurnManager turnManager;

    // Drop được xử lý trong CardDrag.OnEndDrag (đã raycast quái mục tiêu).
    // BattleZone giữ IDropHandler để không chặn drop nhưng không tự chơi bài.
    public void OnDrop(PointerEventData eventData)
    {
    }
}