using System.Collections.Generic;
using UnityEngine;

public static class RuntimeCardLibrary
{
    private static List<CardData> _pool;

    public static CardData GetRandomCard()
    {
        List<CardData> pool = GetCards();
        if (pool.Count == 0) return null;
        return Object.Instantiate(pool[Random.Range(0, pool.Count)]);
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
        return c;
    }
}
