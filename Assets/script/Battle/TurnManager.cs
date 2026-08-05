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

    public static event System.Action<int> PlayerTurnStarted;
    public int TurnCount { get; private set; }

    private bool firstRoundDone = false;

    private bool turnStarted = false;

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        TurnCount++;
        ChangeTurn(TurnState.PlayerTurn);
        battleManager.StartPlayerTurn();
        PlayerTurnStarted?.Invoke(TurnCount);

        // Ice Cream: giữ năng lượng chưa dùng giữa các lượt
        if (turnStarted && RelicManager.Owns("Ice Cream"))
            energyManager.GainEnergy(energyManager.MaxEnergy);
        else
            energyManager.ResetEnergy();

        // Energy đầu trận (Coffee Dripper, Ectoplasm, Tea Set)
        if (!turnStarted)
        {
            int bonus = RelicManager.GetBattleStartEnergyBonus();
            if (bonus > 0)
                energyManager.GainEnergy(bonus);
        }

        turnStarted = true;

        RelicManager.EmitPlayerTurnStart();

        Debug.Log($"--- Player Turn --- Draw:{deckManager.DrawPileCount} Discard:{deckManager.DiscardPileCount}");

        List<CardData> cards = deckManager.DrawCards(drawPerTurn);

        foreach (CardData card in cards)
        {
            Debug.Log($"Drew: {card.cardName}");
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
        RelicManager.EmitPlayerTurnEnd();

        List<CardDisplay> cards =
        new List<CardDisplay>(handManager.GetCardsInHand());
        foreach (CardDisplay card in cards)
        {
            deckManager.AddToDiscard(card.CardData);
            handManager.RemoveCard(card);
        }

        if (!firstRoundDone)
        {
            firstRoundDone = true;
            InjectComplexCards();
        }

        StartEnemyTurn();
    }

    private void InjectComplexCards()
    {
        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        if (db == null) return;

        List<CardData> toAdd = db.GetComplexCards();
        if (toAdd.Count == 0)
        {
            CardData c = db.GetRandomCard();
            if (c == null) return;
            toAdd.Add(c);
        }

        foreach (CardData card in toAdd)
        {
            deckManager.AddCardToDeck(card);
            Debug.Log($"Added to deck: {card.cardName}");
        }
        deckManager.ShuffleDrawPile();
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
