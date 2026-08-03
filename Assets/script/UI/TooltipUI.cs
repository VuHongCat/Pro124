using UnityEngine;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    [Header("Tooltip Panel")]
    public GameObject panel;

    [Header("Text")]
    public TMP_Text nameText;
    public TMP_Text rarityText;
    public TMP_Text descriptionText;

    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        rectTransform = panel.GetComponent<RectTransform>();

        panel.SetActive(false);

        Debug.Log("TooltipUI Ready");
    }

    //====================================================
    // RELIC TOOLTIP
    //====================================================

    public void ShowRelic(RelicData relic)
    {
        panel.SetActive(true);

        // Relic Name
        nameText.text = relic.relicName;
        nameText.color = new Color(0.2f, 1f, 0.2f);

        // Relic Rarity
        rarityText.text = relic.rarity.ToString();

        switch (relic.rarity)
        {
            case RelicRarity.Common:
                rarityText.color = new Color(0.2f, 0.6f, 1f);
                break;

            case RelicRarity.Uncommon:
                rarityText.color = new Color(1f, 0.2f, 0.2f);
                break;

            case RelicRarity.Rare:
                rarityText.color = new Color(1f, 0.85f, 0f);
                break;

            case RelicRarity.Boss:
                rarityText.color = new Color(1f, 0.5f, 0f);
                break;
        }

        // Description
        descriptionText.text = relic.description;

        UpdatePosition();

        Debug.Log("Show Relic Tooltip: " + relic.relicName);
    }

    //====================================================
    // CARD TOOLTIP
    //====================================================

    public void ShowCard(CardData card)
    {
        panel.SetActive(true);

        // Card Name
        nameText.text = card.cardName;
        nameText.color = new Color(0.2f, 1f, 0.2f);

        // Cost + Rarity
        rarityText.text = "Cost: " + card.energyCost + " | " + card.rarity;

        switch (card.rarity)
        {
            case CardRarity.Common:
                rarityText.color = new Color(0.2f, 0.6f, 1f);
                break;

            case CardRarity.Rare:
                rarityText.color = new Color(1f, 0.85f, 0f);
                break;

            case CardRarity.Epic:
                rarityText.color = new Color(0.7f, 0.3f, 1f);
                break;
        }

        // Description
        descriptionText.text = card.description;

        UpdatePosition();

        Debug.Log("Show Card Tooltip: " + card.cardName);
    }

    //====================================================
    // HIDE
    //====================================================

    public void Hide()
    {
        panel.SetActive(false);
    }

    private void Update()
    {
        if (panel.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        rectTransform.position =
            Input.mousePosition + new Vector3(40f, 20f, 0f);
    }
}