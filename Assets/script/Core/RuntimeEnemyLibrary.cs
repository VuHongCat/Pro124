using System.Collections.Generic;
using UnityEngine;

public static class RuntimeEnemyLibrary
{
    private static Dictionary<string, EnemyData> _templates;

    public static List<EnemyData> GetDefaultSequence()
    {
        return new List<EnemyData>
        {
            Build("Slime", EnemyArchetype.Basic, 40, 8, 6, gold: 15),
            Build("Goblin", EnemyArchetype.Poison, 30, 6, 4, poison: 3, gold: 20),
            Build("Bat", EnemyArchetype.Lifesteal, 22, 5, 3, lifesteal: 2, gold: 15)
        };
    }

    private static void EnsureTemplates()
    {
        if (_templates != null)
            return;

        _templates = new Dictionary<string, EnemyData>();
        foreach (EnemyData t in Resources.LoadAll<EnemyData>("Enemies"))
        {
            if (t == null || string.IsNullOrEmpty(t.enemyName))
                continue;
            if (!_templates.ContainsKey(t.enemyName))
                _templates[t.enemyName] = t;
        }
    }

    private static void ApplyVisuals(EnemyData d)
    {
        if (d == null)
            return;

        EnsureTemplates();
        if (_templates.TryGetValue(d.enemyName, out EnemyData t))
        {
            if (d.artwork == null)
                d.artwork = t.artwork;
            if (d.animatorController == null)
                d.animatorController = t.animatorController;
            if (string.IsNullOrEmpty(d.attackStateName))
                d.attackStateName = t.attackStateName;
        }
    }

    public static EnemyData Build(string name, EnemyArchetype archetype, int maxHealth, int attack, int block,
        int poison = 0, int lifesteal = 0, int gold = 0,
        int selfHeal = 0, int regenValue = 0, int buffStrength = 0,
        int weakDamage = 0, int vulnerableDamage = 0, int counterStacks = 0)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.enemyName = name;
        d.archetype = archetype;
        d.maxHealth = maxHealth;
        d.attackDamage = attack;
        d.block = block;
        d.poisonDamage = poison;
        d.lifesteal = lifesteal;
        d.goldReward = gold;
        d.selfHeal = selfHeal;
        d.regenValue = regenValue;
        d.buffStrength = buffStrength;
        d.weakDamage = weakDamage;
        d.vulnerableDamage = vulnerableDamage;
        d.counterStacks = counterStacks;
        ApplyVisuals(d);
        return d;
    }

    // =========================================================
    // PER-MAP ENCOUNTERS
    // =========================================================

    // Số quái xuất hiện đồng thời trên sân theo map
    public static int GetEnemiesPerField(int mapLevel)
    {
        switch (mapLevel)
        {
            case 1: return 1;
            case 2: return 2;
            default: return 3;
        }
    }

    public static List<EnemyData> GetEncounter(int mapLevel)
    {
        List<EnemyData> templates = new(GetMapPool(mapLevel));
        if (templates.Count == 0)
            templates = new(GetMapPool(1));

        int count = GetEnemiesPerField(mapLevel);
        List<EnemyData> encounter = new();

        for (int i = 0; i < count && templates.Count > 0; i++)
        {
            EnemyData t = templates[Random.Range(0, templates.Count)];
            templates.Remove(t);
            encounter.Add(BuildScaled(t, mapLevel));
        }

        return encounter;
    }

    // Template quái cho từng map (base = map 1, sau đó scale)
    private static List<EnemyData> GetMapPool(int mapLevel)
    {
        List<EnemyData> pool = new();

        switch (mapLevel)
        {
            case 1:
                pool.Add(Build("Slime", EnemyArchetype.Basic, 40, 8, 6, gold: 15));
                pool.Add(Build("Goblin", EnemyArchetype.Poison, 30, 6, 4, poison: 3, gold: 20));
                pool.Add(Build("Bat", EnemyArchetype.Lifesteal, 22, 5, 3, lifesteal: 2, gold: 15));
                break;

            case 2:
                pool.Add(Build("Knight", EnemyArchetype.Knight, 55, 8, 12, counterStacks: 3, gold: 30));
                pool.Add(Build("Assassin", EnemyArchetype.Assassin, 35, 14, 0, vulnerableDamage: 3, gold: 25));
                pool.Add(Build("Priest", EnemyArchetype.Priest, 45, 5, 6, selfHeal: 15, regenValue: 4,
                    buffStrength: 2, weakDamage: 3, gold: 25));
                break;

            case 3:
                pool.Add(Build("Golem", EnemyArchetype.Golem, 70, 10, 8, selfHeal: 20, regenValue: 6,
                    buffStrength: 3, gold: 35));
                pool.Add(Build("Knight", EnemyArchetype.Knight, 55, 8, 12, counterStacks: 3, gold: 30));
                pool.Add(Build("Assassin", EnemyArchetype.Assassin, 35, 14, 0, vulnerableDamage: 3, gold: 25));
                break;

            case 4:
                pool.Add(Build("Golem", EnemyArchetype.Golem, 70, 10, 8, selfHeal: 20, regenValue: 6,
                    buffStrength: 3, gold: 35));
                pool.Add(Build("Priest", EnemyArchetype.Priest, 45, 5, 6, selfHeal: 15, regenValue: 4,
                    buffStrength: 2, weakDamage: 3, gold: 25));
                pool.Add(Build("Knight", EnemyArchetype.Knight, 55, 8, 12, counterStacks: 3, gold: 30));
                break;
        }

        return pool;
    }

    // Clone + scale quái theo độ khó map
    public static EnemyData BuildScaled(EnemyData template, int mapLevel, bool isBoss = false)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();

        float hpScale = 1f + (mapLevel - 1) * 0.2f;
        float dmgScale = 1f + (mapLevel - 1) * 0.05f;

        d.enemyName = template.enemyName;
        d.artwork = template.artwork;
        d.archetype = template.archetype;
        d.animatorController = template.animatorController;
        d.attackStateName = template.attackStateName;
        d.maxHealth = Mathf.Max(1, Mathf.RoundToInt(template.maxHealth * hpScale));
        d.attackDamage = Mathf.RoundToInt(template.attackDamage * dmgScale);
        d.block = Mathf.RoundToInt(template.block * dmgScale);
        d.poisonDamage = template.poisonDamage;
        d.lifesteal = template.lifesteal;
        d.selfHeal = Mathf.RoundToInt(template.selfHeal * dmgScale);
        d.regenValue = template.regenValue;
        d.buffStrength = template.buffStrength;
        d.weakDamage = template.weakDamage;
        d.vulnerableDamage = template.vulnerableDamage;
        d.counterStacks = template.counterStacks;
        d.goldReward = template.goldReward + (mapLevel - 1) * 10;
        d.isBoss = isBoss || template.isBoss;
        ApplyVisuals(d);

        return d;
    }
}
