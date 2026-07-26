using UnityEngine;
using UnityEngine.EventSystems;

public class BattleZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private TurnManager turnManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (turnManager.CurrenTurn != TurnManager.TurnState.PlayerTurn)
            return;
        CardDisplay card = eventData.pointerDrag.GetComponent<CardDisplay>();

        if (card == null)
            return;
        Debug.Log(card.CardData.cardName);
        battleManager.PlayCard(card);
    }
}