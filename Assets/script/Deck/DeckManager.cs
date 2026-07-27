using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<CardData> startingDeck = new();

    private List<CardData> drawPile = new();
    private List<CardData> discardPile = new();
    private List<CardData> exhaustPile = new();

    public int DrawPileCount => drawPile.Count;
    public int DiscardPileCount => discardPile.Count;
    public int ExhaustPileCount => exhaustPile.Count;

    public IReadOnlyList<CardData> DrawPile => drawPile;
    public IReadOnlyList<CardData> DiscardPile => discardPile;
    public IReadOnlyList<CardData> ExhaustPile => exhaustPile;

    private void Awake()
    {
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        drawPile.Clear();
        discardPile.Clear();
        exhaustPile.Clear();

        drawPile.AddRange(startingDeck);
        Shuffle(drawPile);
        foreach (CardData card in drawPile)
        {
            Debug.Log(card.cardName);
        }
    }

    private void Shuffle(List<CardData> cards)
    {
        for (int i = cards.Count -1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public CardData DrawCard()
    {
        if (drawPile.Count == 0)
        {
            Reshuffle();
            if (drawPile.Count == 0) return null;
        }

        CardData card = drawPile[0];
        drawPile.RemoveAt(0);

        return card;
    }

    public List<CardData> DrawCards(int amount)
    {
        List<CardData> cards = new();

        for(int i = 0; i < amount; i++)
        {
            CardData card = DrawCard();
            if (card == null) break;
            cards.Add(card);
        }
        return cards;
    }

    public List<CardData> DrawStartingHand()
    {
        return DrawCards(4);
    }

    public void AddToDiscard(CardData card)
    {
        discardPile.Add(card);
    }

    private void Reshuffle()
    {
        if (discardPile.Count == 0) return;

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }
}
