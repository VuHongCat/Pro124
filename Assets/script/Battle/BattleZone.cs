using UnityEngine;
using UnityEngine.EventSystems;

public class BattleZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private BattleManager battleManager;

    public void OnDrop(PointerEventData eventData)
    {
        CardDisplay card = eventData.pointerDrag.GetComponent<CardDisplay>();

        if (card == null)
            return;
        Debug.Log(card.CardData.cardName);
        battleManager.PlayCard(card);
    }
}