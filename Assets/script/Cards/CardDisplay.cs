using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image artworkImage;

    public CardData CardData { get; private set; }

    public void Setup(CardData data)
    {
        CardData = data;

        cardNameText.text = data.isUpgraded ? data.cardName + "+" : data.cardName;
        cardNameText.color = data.isUpgraded ? Color.cyan : Color.white;
        costText.text = data.energyCost.ToString();
        descriptionText.text = data.description;

        if (artworkImage == null)
            return;

        if (data.artwork != null)
        {
            artworkImage.sprite = data.artwork;
            artworkImage.color = Color.white;
        }
        else
        {
            Debug.LogWarning("[CardDisplay] Card '" + data.cardName + "' has no artwork assigned.", this);
            artworkImage.sprite = GetPlaceholderSprite();
            artworkImage.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        }
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
}