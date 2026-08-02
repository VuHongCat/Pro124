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
}

public enum CardPool { Basic, Complex }
