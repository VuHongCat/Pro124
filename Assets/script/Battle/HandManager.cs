using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handPanel;
    [SerializeField] private CardFactory cardFactory;
    [SerializeField] private HandLayout handLayout;
    [SerializeField] private int maxHandSize = 10;

    private List<CardDisplay> cardsInHand = new();
    public IReadOnlyList<CardDisplay> Cards => cardsInHand;
    public bool IsFull => cardsInHand.Count >= maxHandSize;

    public void AddCard(CardData data)
    {
        if (IsFull) return;
        GameObject cardObject = cardFactory.CreateCard(data, handPanel);
        CardDisplay display = cardObject.GetComponent<CardDisplay>();
        cardsInHand.Add(display);
        handLayout.UpdateLayout();
    }

    public void RemoveCard(CardDisplay card)
    {
        if (cardsInHand.Remove(card))
        {
            Destroy(card.gameObject);
            handLayout.UpdateLayout();
        }
    }

    public void ClearHand()
    {
        foreach(CardDisplay card in cardsInHand)
        {
            Destroy(card.gameObject);
        }
        cardsInHand.Clear();
        handLayout.UpdateLayout();
    }
}
