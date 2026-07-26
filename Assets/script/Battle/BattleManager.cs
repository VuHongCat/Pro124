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
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private EnemyData slime;
    [SerializeField] private Transform enemyArea;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private CardEffectResolver effectResolver;
    [SerializeField] private PlayerHealth playerHealth;
    private EnemyCombat enemyCombat;

    private void Start()
    {
        GameObject enemy = enemyFactory.CreateEnemy(slime, enemyArea);
        enemyCombat = enemy.GetComponent<EnemyCombat>();
        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth.OnEnemyDeath += OnEnemyDeath;
        enemyCombat.DecideNextIntent();
    }

    public void PlayCard(CardDisplay card)
    {
        if (!energyManager.HasEnoughEnergy(card.CardData.energyCost)) return;
        energyManager.SpendEnergy(card.CardData.energyCost);
        effectResolver.Resolve(card.CardData, enemyHealth);
        deckManager.AddToDiscard(card.CardData);
        handManager.RemoveCard(card);
    }

    private void OnEnemyDeath(EnemyHealth enemy)
    {
        Debug.Log("Battle Won!");
    }

    public void EnemyAttack()
    {
        if (enemyCombat == null) return;
        enemyCombat.Attack(playerHealth);
    }
}
