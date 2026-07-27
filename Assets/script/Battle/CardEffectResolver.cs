using System;
using UnityEngine;

public class CardEffectResolver : MonoBehaviour
{
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;
    public void Resolve(CardData card, EnemyHealth target)
    {
        switch (card.cardType)
        {
            case CardType.Attack:
                ResolveAttack(card, target);
                break;
            case CardType.Block:
                ResolveBlock(card, target);
                break;
            case CardType.Heal:
                ResolveHeal(card, target);
                break;
        }
    }

    private void ResolveAttack(CardData card, EnemyHealth target)
    {
        playerCombat.Attack(target, card.damage);
    }
    private void ResolveBlock(CardData card, EnemyHealth target)
    {
        playerBlock.AddBlock(card.block);
    }
    private void ResolveHeal(CardData card, EnemyHealth target)
    {
        Debug.Log("Heal Card");
    }
}
