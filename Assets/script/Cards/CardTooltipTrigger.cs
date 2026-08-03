using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardTooltipTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public CardData card;

    [Header("Card Artwork")]
    public Image artwork;

    public void SetCard(CardData data)
    {
        card = data;

        if (artwork != null && card != null)
        {
            artwork.sprite = card.artwork;
        }

        Debug.Log("Đã gán card: " + card.cardName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (card == null)
        {
            Debug.LogWarning("CardData NULL");
            return;
        }

        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.ShowCard(card);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Hide();
        }
    }
}