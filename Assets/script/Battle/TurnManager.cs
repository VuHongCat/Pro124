using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    }
    [SerializeField] private int drawPerTurn = 5;

    [SerializeField] private TurnState currentTurn;
    [SerializeField] private DeckManager deckManager;

    [SerializeField] private HandManager handManager;

    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private BattleManager battleManager;
    public TurnState CurrenTurn => currentTurn;

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        ChangeTurn(TurnState.PlayerTurn);
        battleManager.StartPlayerTurn();
        energyManager.ResetEnergy();
        List<CardData> cards = deckManager.DrawCards(drawPerTurn);

        foreach (CardData card in cards)
        {
            handManager.AddCard(card);
        }
    }

    public void StartEnemyTurn()
    {
        ChangeTurn(TurnState.EnemyTurn);

        Invoke(nameof(FinishEnemyTurn), 1f);
    }

    public void EndPlayerTurn()
    {
        List<CardDisplay> cards =
        new List<CardDisplay>(handManager.GetCardsInHand());
        foreach (CardDisplay card in cards)
        {
            deckManager.AddToDiscard(card.CardData);
            handManager.RemoveCard(card);
        }
        StartEnemyTurn();
    }
    private void FinishEnemyTurn()
    {
        battleManager.EnemyAttack();
        Invoke(nameof(StartNextPlayerTurn), 0.5f);
    }
    private void StartNextPlayerTurn()
    {
        StartPlayerTurn();
    }
    public void EndTurn()
    {
        if (currentTurn != TurnState.PlayerTurn)
            return;

        EndPlayerTurn();
    }
    private void ChangeTurn(TurnState state)
    {
        currentTurn = state;
        Debug.Log(currentTurn);
    }
}
