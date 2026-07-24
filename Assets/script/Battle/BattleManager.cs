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

    private void Start()
    {
        GameObject enemy = enemyFactory.CreateEnemy(slime, enemyArea);
        enemyHealth = enemy.GetComponent<EnemyHealth>();
    }
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemyHealth.TakeDamage(8);
        }
    }

    public void PlayCard(CardDisplay card)
    {
        if (!energyManager.HasEnoughEnergy(card.CardData.energyCost)) return;
        energyManager.SpendEnergy(card.CardData.energyCost);
        effectResolver.Resolve(card.CardData, enemyHealth);
        deckManager.AddToDiscard(card.CardData);
        handManager.RemoveCard(card);
    }
}
