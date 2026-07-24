using System;
using UnityEngine;

public class CardEffectResolver : MonoBehaviour
{
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
        target.TakeDamage(card.damage);
    }
    private void ResolveBlock(CardData card, EnemyHealth target)
    {
        Debug.Log("Block Card");
    }
    private void ResolveHeal(CardData card, EnemyHealth target)
    {
        Debug.Log("Heal Card");
    }
}
