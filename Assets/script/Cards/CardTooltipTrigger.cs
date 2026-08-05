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
            if (card.artwork != null)
            {
                artwork.sprite = card.artwork;
                artwork.color = Color.white;
            }
            else
            {
                Debug.LogWarning("[CardTooltipTrigger] Card '" + card.cardName + "' has no artwork assigned.", this);
                artwork.sprite = GetPlaceholderSprite();
                artwork.color = new Color(0.1f, 0.1f, 0.12f, 1f);
            }
        }

        Debug.Log("Assigned card: " + card.cardName);
    }

    private static Sprite placeholderSprite;

    private static Sprite GetPlaceholderSprite()
    {
        if (placeholderSprite == null)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            placeholderSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            placeholderSprite.name = "CardArtworkPlaceholder";
        }

        return placeholderSprite;
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