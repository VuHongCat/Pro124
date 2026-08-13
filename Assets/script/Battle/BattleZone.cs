using UnityEngine;
using UnityEngine.EventSystems;

public class BattleZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private TurnManager turnManager;

    // Drop is handled in CardDrag.OnEndDrag (already raycasts the target enemy).
    // BattleZone keeps IDropHandler so it doesn't block drops but doesn't play cards itself.
    public void OnDrop(PointerEventData eventData)
    {
    }
}