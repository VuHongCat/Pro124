using System.Collections.Generic;
using UnityEngine;

public static class CurseLibrary
{
    private static List<CardData> _curses;

    public static List<CardData> GetCurses()
    {
        if (_curses == null) Build();
        return new List<CardData>(_curses);
    }

    public static CardData GetRandomCurse()
    {
        List<CardData> curses = GetCurses();
        if (curses.Count == 0) return null;
        return Object.Instantiate(curses[Random.Range(0, curses.Count)]);
    }

    private static void Build()
    {
        _curses = new List<CardData>();
        _curses.Add(Build("Curse of Weakness", "Apply 3 Weak to yourself.", 0, StatusType.Weak, 3, 2));
        _curses.Add(Build("Curse of Frailty", "Apply 3 Vulnerable to yourself.", 0, StatusType.Vulnerable, 3, 2));
        _curses.Add(Build("Curse of Decay", "Apply 6 Bleed to yourself.", 0, StatusType.Bleed, 6, 1));
        _curses.Add(Build("Curse of Plague", "Apply 5 Poison to yourself.", 0, StatusType.Poison, 5, 1));
        _curses.Add(Build("Curse of Agony", "Take 6 damage.", 0, StatusType.None, 0, 1, 6));
        _curses.Add(Build("Curse of Greed", "Lose 25 gold.", 0, StatusType.None, 25, 1));
        _curses.Add(Build("Curse of Fatigue", "Lose 2 Energy.", 0, StatusType.None, 2, 1));
        _curses.Add(Build("Curse of Misfortune", "Apply 2 Weak and 2 Vulnerable to yourself.", 0, StatusType.None, 2, 2));
        _curses.Add(Build("Curse of Torment", "Take 4 damage, apply 3 Bleed to yourself.", 0, StatusType.Bleed, 3, 1, 4));
        _curses.Add(Build("Curse of Despair", "Take 5 damage, apply 3 Vulnerable to yourself.", 0, StatusType.Vulnerable, 3, 2, 5));
    }

    private static CardData Build(string name, string desc, int cost, StatusType status, int amount, int duration, int selfDamage = 0)
    {
        CardData c = ScriptableObject.CreateInstance<CardData>();
        c.cardName = name;
        c.description = desc;
        c.energyCost = cost;
        c.cardType = CardType.Curse;
        c.target = CardTarget.Self;
        c.applyStatus = status;
        c.statusAmount = amount;
        c.statusDuration = duration;
        c.damage = selfDamage;
        c.rarity = CardRarity.Rare;
        c.pool = CardPool.Basic;
        ApplyArtwork(c);
        return c;
    }

    private static void ApplyArtwork(CardData c)
    {
        if (c == null) return;
        foreach (CardData loaded in Resources.LoadAll<CardData>("Cards"))
        {
            if (loaded == null || loaded.artwork == null) continue;
            if (loaded.cardName == c.cardName)
            {
                c.artwork = loaded.artwork;
                return;
            }
        }
    }
}
