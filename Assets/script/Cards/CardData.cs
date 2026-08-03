using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    [Header("Basic")]
    public string cardName;
    [TextArea]
    public string description;
    public Sprite artwork;
    public int energyCost;
    public CardType cardType;
    public CardTarget target;
    public CardRarity rarity;
    [Header("Value")]
    public int damage;
    public int block;
    public int strength;

    [Header("Heal")]
    public int heal;

    [Header("Status")]
    public StatusType applyStatus;
    public int statusAmount;
    public int statusDuration = 1;

    [Header("Pool")]
    public CardPool pool = CardPool.Basic;

    [Header("Upgrade")]
    public bool isUpgraded;

    public void Upgrade()
    {
        if (isUpgraded) return;
        if (damage > 0) damage = Mathf.CeilToInt(damage * 1.25f);
        if (block > 0) block = Mathf.CeilToInt(block * 1.25f);
        if (strength > 0) strength += 1;
        if (heal > 0) heal = Mathf.CeilToInt(heal * 1.3f);
        if (statusAmount > 0) statusAmount += 1;
        isUpgraded = true;
    }
}

public enum CardPool { Basic, Complex }
