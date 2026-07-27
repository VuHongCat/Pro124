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

        cardNameText.text = data.cardName;
        costText.text = data.energyCost.ToString();
        descriptionText.text = data.description;

        if (artworkImage != null)
            artworkImage.sprite = data.artwork;
    }
}