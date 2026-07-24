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
}
