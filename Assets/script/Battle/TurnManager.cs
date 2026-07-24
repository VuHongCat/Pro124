using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    }

    [SerializeField] private TurnState currentTurn;
    [SerializeField] private DeckManager deckManager;

    [SerializeField] private HandManager handManager;

    [SerializeField] private EnergyManager energyManager;
    public TurnState CurrenTurn => currentTurn;

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        currentTurn = TurnState.PlayerTurn;
        energyManager.ResetEnergy();
        List<CardData> cards = deckManager.DrawCards(4);

        foreach (CardData card in cards)
        {
            handManager.AddCard(card);
        }
        Debug.Log("PLAYER TURN");
    }

    public void StartEnemyTurn()
    {
        currentTurn = TurnState.EnemyTurn;

        Debug.Log("ENEMY TURN");
        Invoke(nameof(FinishEnemyTurn), 1f);
    }

    public void EndPlayerTurn()
    {
        Debug.Log("PLAYER END TURN");
        foreach (CardDisplay card in handManager.GetCardsInHand())
        {
            deckManager.AddToDiscard(card.CardData);
            handManager.RemoveCard(card);
        }
        StartEnemyTurn();
    }
    private void FinishEnemyTurn()
    {
        Debug.Log("Enemy END TURN");
        StartPlayerTurn();
    }

    public void EndTurn()
    {
        if (currentTurn != TurnState.PlayerTurn)
            return;

        EndPlayerTurn();
    }
}
