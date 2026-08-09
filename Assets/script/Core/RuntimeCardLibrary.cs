using System.Collections.Generic;
using UnityEngine;

public static class RuntimeCardLibrary
{
    private static List<CardData> _pool;
    private static Dictionary<string, Sprite> artworkByName;

    private static void EnsureArtworkCache()
    {
        if (artworkByName != null)
            return;

        artworkByName = new Dictionary<string, Sprite>();
        foreach (CardData c in Resources.LoadAll<CardData>("Cards"))
        {
            if (c == null || c.artwork == null || string.IsNullOrEmpty(c.cardName))
                continue;
            if (!artworkByName.ContainsKey(c.cardName))
                artworkByName[c.cardName] = c.artwork;
        }
    }

    private static void ApplyArtwork(CardData c)
    {
        if (c == null)
            return;

        EnsureArtworkCache();
        if (artworkByName.TryGetValue(c.cardName, out Sprite art))
            c.artwork = art;
    }

    public static CardData GetRandomCard()
    {
        List<CardData> pool = GetCards();
        if (pool.Count == 0) return null;
        return Object.Instantiate(pool[Random.Range(0, pool.Count)]);
    }

    public static CardData GetCardByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        CardData[] assets = Resources.LoadAll<CardData>("Cards");
        foreach (CardData c in assets)
        {
            if (c != null && c.cardName == name)
                return c;
        }

        foreach (CardData c in GetCards())
        {
            if (c != null && c.cardName == name)
                return c;
        }

        foreach (CardData c in GetStarterDeck())
        {
            if (c != null && c.cardName == name)
                return c;
        }

        return null;
    }

    public static List<CardData> GetCards()
    {
        if (_pool == null) BuildPool();
        return new List<CardData>(_pool);
    }

    private static void BuildPool()
    {
        _pool = new List<CardData>();
        _pool.Add(Build("HeavyBlade", "Deal heavy damage.", 2, 14, 0, 0));
        _pool.Add(Build("Guardian", "Gain block.", 1, 0, 12, 0));
        _pool.Add(Build("Bloodthirst", "Deal damage and heal 50% of damage dealt.", 2, 10, 0, 0));
        _pool.Add(Build("Counter Stance", "Reflect 60% of damage for 2 turns.", 1, 0, 0, 0));
        _pool.Add(Build("Intimidate", "Reduce enemy damage.", 1, 0, 0, 0));
        _pool.Add(Build("Hemorrhage", "Make the enemy bleed.", 1, 0, 0, 0));
        _pool.Add(Build("Enrage", "Gain +3 Strength.", 2, 0, 0, 3));
        _pool.Add(Build("Risky Gambit", "Lose 5 HP, draw 2 extra cards.", 0, 0, 0, 0));
        _pool.Add(Build("Flurry", "Deal 3 damage 3 times.", 1, 3, 0, 0));
        _pool.Add(Build("Power Swing", "Deal 4 damage +2 per Strength.", 2, 4, 0, 0));
        _pool.Add(Build("Bastion", "Gain 1 block for each card in your discard pile.", 1, 0, 0, 0));
        _pool.Add(Build("Rend", "Deal 6 damage, then deal 2 per Bleed stack.", 1, 6, 0, 0));
    }

    public static List<CardData> GetStarterDeck()
    {
        List<CardData> deck = new();
        for (int i = 0; i < 5; i++)
            deck.Add(BuildStarter("Strike", "Deal 9 damage.", 1, 9, 0, 0));
        for (int i = 0; i < 3; i++)
            deck.Add(BuildStarter("Defend", "Gain 8 block.", 1, 0, 8, 0));
        deck.Add(BuildStarter("Bash", "Deal 8 damage to all enemies.", 2, 8, 0, 0));
        deck.Add(BuildStarter("Second Wind", "Heal 10 HP, +30% more if below 30% HP.", 1, 0, 10, 0));
        return deck;
    }

    private static CardData BuildStarter(string name, string desc, int cost, int dmg, int heal, int str)
    {
        CardData c = ScriptableObject.CreateInstance<CardData>();
        c.cardName = name;
        c.description = desc;
        c.energyCost = cost;
        c.damage = dmg;
        c.heal = heal;
        c.strength = str;
        c.rarity = CardRarity.Common;
        c.pool = CardPool.Basic;
        ApplyArtwork(c);
        return c;
    }

    private static CardData Build(string name, string desc, int cost, int dmg, int blk, int str)
    {
        CardData c = ScriptableObject.CreateInstance<CardData>();
        c.cardName = name;
        c.description = desc;
        c.energyCost = cost;
        c.damage = dmg;
        c.block = blk;
        c.strength = str;
        c.rarity = CardRarity.Rare;
        c.pool = CardPool.Complex;
        ApplyArtwork(c);
        return c;
    }
}
