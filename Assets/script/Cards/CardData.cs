using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Objects/CardData")]
public class CardData : ScriptableObject
{
    public GameObject attackVFX;
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

        switch (cardName.Trim())
        {
            case "Strike":              damage = 12; description = "Deal 12 damage"; break;
            case "Bash":                damage = 11; description = "Deal 11 damage to all enemies"; break;
            case "HeavyBlade":          damage = 14; description = "Deal 14 damage"; break;
            case "Combo":               damage = 6;  description = "Deal 6 damage twice"; break;
            case "Chain Hit":           damage = 8;  description = "Deal 8 damage, lower card cost by 1"; break;
            case "Counter Stance":      statusAmount = 3; break;
            case "Last Stand":          damage = 14; description = "Deal 14 damage, if player health below 50%, deal 28 damage instead"; break;
            case "Sacrifice":           damage = 26; description = "Deal 26 damage, deal 13 damage to yourself"; break;
            case "Bloodthirst":         damage = 11; description = "Deal 11 damage, recover your health equal 50% of your damage hit the enemy"; break;
            case "Executioner":         statusAmount = 30; description = "Execute enemy if below 30% health (not apply to boss)"; break;
            case "Blade Storm":         damage = 26; description = "Deal 26 damage to all enemies, if kill at least 2 enemies, get 1 random card"; break;
            case "Defend":              block = 11;  description = "Gain 11 Block"; break;
            case "Guardian":            block = 16;  description = "Block 16 damage"; break;
            case "Blood Barrier":       block = 13;  description = "Gain 13 block, gain more block equal by 10% of your lost health"; break;
            case "Steel Skin":          block = 9;   description = "Gain 9 block, decrease 50% of enemy damage to player"; break;
            case "Undying Will":        block = 34;  break;
            case "Second Wind":         heal = 13;   description = "Heal 13 HP, if player health below 30%, heal 30% more HP"; break;
            case "Blood Feast":         heal = 11;   description = "Heal 11 HP, your next attack will heal equal of 50% of your damage"; break;
            case "Rejuvenating Aura":   heal = 40; statusAmount = 10; description = "Heal 40 HP, heal 10 more HP in 2 turns"; break;
            case "Enrage":              statusAmount = 5; description = "Gain strength effect, each effect gives you 5 more damage (stackable)"; break;
            case "Shockwave":           statusAmount = 5; break;
            case "Shatter Armor":       statusAmount = 3; break;
            case "Intimidate":          statusAmount = 8; break;
            case "Hemorrhage":          statusAmount = 9; description = "Give bleeding effect, lose 9 HP each turn, if player use attack card, bleeding damage more"; break;
            case "Refresh":             statusAmount = 2; description = "Discard 1 card on your hand and then draw 2 new cards from your deck"; break;
            case "Risky Gambit":        damage = 3;  description = "Deal 3 damage to yourself, draw 2 cards"; break;
            case "Double Edge":         damage = 6;  description = "Deal 6 damage twice"; break;
            case "Whirlwind":           damage = 12; description = "Deal 12 damage to all enemies"; break;
            case "Puncture":            damage = 5; statusAmount = 3; description = "Deal 5 damage, apply 3 Bleed"; break;
            case "Shield Bash":         block = 8;  damage = 6; description = "Gain 8 Block, deal 6 damage"; break;
            case "Vampiric Strike":     damage = 7;  description = "Deal 7 damage, heal 100% of damage dealt"; break;
            case "Crushing Blow":       damage = 14; description = "Deal 14 damage, double if enemy is Vulnerable"; break;
            case "Assassinate":         damage = 6;  description = "Deal 6 damage, deal 18 instead if enemy has Bleed"; break;
            case "Poison Dagger":       damage = 3; statusAmount = 6; description = "Deal 3 damage, apply 6 Poison"; break;
            case "Blood Boil":          damage = 22; statusAmount = 5; description = "Lose 5 HP, deal 22 damage"; break;
            case "Fury":                damage = 11; statusAmount = 5; description = "Deal 11 damage, gain 5 Strength"; break;
            case "Iron Wall":           block = 11; description = "Gain 11 Block"; break;
            case "Brace":               block = 8;  description = "Gain 8 Block, draw 1 card"; break;
            case "Reposition":          block = 6;  description = "Gain 6 Block"; break;
            case "Mirror Shield":       block = 11; statusAmount = 3; description = "Gain 11 Block, gain 3 Counter"; break;
            case "Fortify":             block = 9;  description = "Gain 9 Block, gain 18 instead if HP below 50%"; break;
            case "Stoneskin":           block = 14; statusAmount = 2; description = "Gain 14 Block, +2 Block per Strength"; break;
            case "Aegis":               block = 28; statusAmount = 8; description = "Gain 28 Block, gain 8 Regen"; break;
            case "Bandage":             heal = 8;  block = 5; description = "Heal 8 HP, gain 5 Block"; break;
            case "Leech":               damage = 8; description = "Deal 8 damage, heal 100% of damage dealt"; break;
            case "Life Spring":         heal = 6;  statusAmount = 6; description = "Heal 6 HP, gain 6 Regen"; break;
            case "Greater Heal":        heal = 21; description = "Heal 21 HP"; break;
            case "Absorb":              heal = 11; block = 11; description = "Heal 11 HP, gain 11 Block"; break;
            case "Vampiric Aura":       statusAmount = 7; description = "Gain 7 Lifesteal"; break;
            case "Weaken":              statusAmount = 3; description = "Apply 3 Weak to enemy"; break;
            case "Mark Target":         statusAmount = 6; description = "Apply 6 Vulnerable to enemy"; break;
            case "Bleed Out":           statusAmount = 7; description = "Apply 7 Bleed to enemy"; break;
            case "Venom":               statusAmount = 8; description = "Apply 8 Poison to enemy"; break;
            case "Debilitate":          statusAmount = 4; description = "Apply 4 Weak and 4 Vulnerable to enemy"; break;
            case "Adrenaline":          statusAmount = 4; description = "Draw 4 cards, lose 2 HP"; break;
            case "Second Chance":       statusAmount = 3; description = "Gain 3 Immortal"; break;
            default:
                UpgradeGeneric();
                break;
        }

        isUpgraded = true;
    }

    private void UpgradeGeneric()
    {
        if (damage > 0) damage = Mathf.CeilToInt(damage * 1.25f);
        if (block > 0) block = Mathf.CeilToInt(block * 1.25f);
        if (strength > 0) strength += 1;
        if (heal > 0) heal = Mathf.CeilToInt(heal * 1.3f);
        if (statusAmount > 0) statusAmount += 1;
    }

    [Header("Shop")]
    public int shopPrice = 50;
}


public enum CardPool
{
    Basic,
    Complex
}
