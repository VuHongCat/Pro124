using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private CardDatabase database;
    [SerializeField] private HandManager handManager;
    [SerializeField] private DeckManager deckManager;

    private void Start()
    {
        List<CardData> cards = deckManager.DrawStartingHand();
        foreach(CardData card in cards)
        {
            handManager.AddCard(card);
        }
    }
}
