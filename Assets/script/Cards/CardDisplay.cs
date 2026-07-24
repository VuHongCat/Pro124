using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField] private CardData cardData;
    [Header("UI")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;

    [SerializeField] private Image artworkImage;

    private void Start()
    {
        if (cardData != null)
        {
            UpdateCardUI();
        }
    }

    public void SetCard(CardData data)
    {
        cardData = data;
        UpdateCardUI();
    }

    private void UpdateCardUI()
    {
        cardNameText.text = cardData.name;
        costText.text = cardData.energyCost.ToString();
        descriptionText.text = cardData.description;
        artworkImage.sprite = cardData.artwork;
    }
}
