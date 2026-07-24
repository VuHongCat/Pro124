using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private CardDatabase database;
    [SerializeField] private HandManager handManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private EnergyManager energyManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            energyManager.SpendEnergy(1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            energyManager.ResetEnergy();
        }
    }

    public void PlayCard(CardDisplay card)
    {
        deckManager.AddToDiscard(card.CardData);
        handManager.RemoveCard(card);
    }
}
