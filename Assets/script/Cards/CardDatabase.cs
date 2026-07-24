using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    [SerializeField] private List<CardData> allCards = new();

    private Dictionary<string, CardData> cardLookup;

    public IReadOnlyList<CardData> AllCards => allCards;

    private void Awake()
    {
        cardLookup = new Dictionary<string, CardData>();

        foreach (CardData card in allCards)
        {
            if (!cardLookup.ContainsKey(card.cardName))
            {
                cardLookup.Add(card.cardName, card);
            }
            else
            {
                Debug.LogWarning($"Trùng tên Card: {card.cardName}");
            }
        }
    }

    public CardData GetCard(string name)
    {
        cardLookup.TryGetValue(name, out CardData card);
        return card;
    }
}