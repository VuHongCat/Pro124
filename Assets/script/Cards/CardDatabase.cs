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
                cardLookup.Add(card.cardName, card);
            else
                Debug.LogWarning($"Duplicate card name: {card.cardName}");
        }
    }

    public CardData GetCard(string name)
    {
        if (cardLookup == null) return null;
        cardLookup.TryGetValue(name, out CardData card);
        return card;
    }

    public List<CardData> GetStarterDeck()
    {
        List<CardData> deck = new();
        string[] starterNames =
        {
            "Strike", "Strike", "Strike", "Strike", "Strike",
            "Defend", "Defend", "Defend", "Defend",
            "Bash"
        };

        foreach (string cardName in starterNames)
        {
            CardData card = FindCard(cardName);
            if (card == null)
            {
                Debug.LogWarning($"[CardDatabase] Starter card not found: {cardName}");
                continue;
            }
            deck.Add(Instantiate(card));
        }

        return deck;
    }

    private CardData FindCard(string cardName)
    {
        if (cardLookup != null &&
            cardLookup.TryGetValue(cardName, out CardData cached))
            return cached;

        foreach (CardData card in allCards)
            if (card != null && card.cardName == cardName)
                return card;

        return null;
    }

    public List<CardData> GetComplexCards()
    {
        List<CardData> complex = new();
        foreach (CardData card in allCards)
            if (card.pool == CardPool.Complex)
                complex.Add(Instantiate(card));
        return complex;
    }

    public CardData GetRandomCard()
    {
        if (allCards.Count == 0) return null;
        return Instantiate(allCards[Random.Range(0, allCards.Count)]);
    }
}
