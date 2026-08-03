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
        _pool.Add(Build("HeavyBlade", "Đòn nặng gây sát thương lớn.", 2, 14, 0, 0));
        _pool.Add(Build("Guardian", "Chặn đòn hiệu quả.", 1, 0, 12, 0));
        _pool.Add(Build("Bloodthirst", "Gây sát thương và hồi máu 50% sát thương gây ra.", 2, 10, 0, 0));
        _pool.Add(Build("Counter Stance", "Phản đòn 60% sát thương trong 2 lượt.", 1, 0, 0, 0));
        _pool.Add(Build("Intimidate", "Giảm sát thương kẻ địch.", 1, 0, 0, 0));
        _pool.Add(Build("Hemorrhage", "Gây chảy máu kẻ địch.", 1, 0, 0, 0));
        _pool.Add(Build("Enrage", "Tăng sức mạnh +3.", 2, 0, 0, 3));
        _pool.Add(Build("Risky Gambit", "Mất 5 HP, rút thêm 2 lá.", 0, 0, 0, 0));
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
