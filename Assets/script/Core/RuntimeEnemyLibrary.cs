using System.Collections.Generic;
using UnityEngine;

public static class RuntimeEnemyLibrary
{
    private static Dictionary<string, EnemyData> _templates;

    public static List<EnemyData> GetDefaultSequence()
    {
        return new List<EnemyData>
        {
            Build("Slime", EnemyArchetype.Basic, 40, 8, 6, gold: 15,
                role: "Pure basic attacker that SPLITS on death: killing it spawns 2 smaller Slimes. "
                    + "Clear the small ones fast so they don't pile up.",
                canSplit: true),
            Build("Goblin", EnemyArchetype.Poison, 30, 6, 4, poison: 3, gold: 20,
                role: "Applies Poison on every hit, which ignores your block. Kill it fast before the poison stacks up."),
            Build("Bat", EnemyArchetype.Lifesteal, 22, 5, 3, lifesteal: 2, gold: 15,
                role: "Heals itself for the damage it deals (Lifesteal). Block its attacks to limit its healing.")
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
        int weakDamage = 0, int vulnerableDamage = 0, int counterStacks = 0,
        string role = "",
        bool canSplit = false, int splitCount = 2, float splitHpScale = 0.5f, int splitDmgScale = 1)
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
        d.role = role;
        d.canSplit = canSplit;
        d.splitCount = splitCount;
        d.splitHpScale = splitHpScale;
        d.splitDmgScale = splitDmgScale;
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
                pool.Add(Build("Slime", EnemyArchetype.Basic, 40, 8, 6, gold: 15,
                    role: "Pure basic attacker that SPLITS on death: killing it spawns 2 smaller Slimes. "
                        + "Clear the small ones fast so they don't pile up.",
                    canSplit: true));
                pool.Add(Build("Goblin", EnemyArchetype.Poison, 30, 6, 4, poison: 3, gold: 20,
                    role: "Applies Poison on every hit, which ignores your block. Kill it fast before the poison stacks up."));
                pool.Add(Build("Bat", EnemyArchetype.Lifesteal, 22, 5, 3, lifesteal: 2, gold: 15,
                    role: "Heals itself for the damage it deals (Lifesteal). Block its attacks to limit its healing."));
                break;

            case 2:
                pool.Add(Build("Knight", EnemyArchetype.Knight, 55, 8, 12, counterStacks: 3, gold: 30,
                    role: "High block plus Counter: hitting it lets it strike back. Break its shield or use ignore-block damage."));
                pool.Add(Build("Assassin", EnemyArchetype.Assassin, 35, 14, 0, vulnerableDamage: 3, gold: 25,
                    role: "No block but very high damage, and applies Vulnerable to you. Top priority target - kill it early."));
                pool.Add(Build("Priest", EnemyArchetype.Priest, 45, 5, 6, selfHeal: 15, regenValue: 4,
                    buffStrength: 2, weakDamage: 3, gold: 25,
                    role: "Buffs its allies with random stats (Strength/Regen/Counter/Block) 70% of the time and attacks the rest. "
                        + "Kill it early before its team snowballs."));
                break;

            case 3:
                pool.Add(Build("Golem", EnemyArchetype.Golem, 70, 10, 8, selfHeal: 20, regenValue: 6,
                    buffStrength: 3, gold: 35,
                    role: "High HP and block, strong self-heal, Regen and Strength buffs. The longer the fight, the more dangerous it gets."));
                pool.Add(Build("Knight", EnemyArchetype.Knight, 55, 8, 12, counterStacks: 3, gold: 30,
                    role: "High block plus Counter: hitting it lets it strike back. Break its shield or use ignore-block damage."));
                pool.Add(Build("Assassin", EnemyArchetype.Assassin, 35, 14, 0, vulnerableDamage: 3, gold: 25,
                    role: "No block but very high damage, and applies Vulnerable to you. Top priority target - kill it early."));
                break;

            case 4:
                pool.Add(Build("Golem", EnemyArchetype.Golem, 70, 10, 8, selfHeal: 20, regenValue: 6,
                    buffStrength: 3, gold: 35,
                    role: "High HP and block, strong self-heal, Regen and Strength buffs. The longer the fight, the more dangerous it gets."));
                pool.Add(Build("Priest", EnemyArchetype.Priest, 45, 5, 6, selfHeal: 15, regenValue: 4,
                    buffStrength: 2, weakDamage: 3, gold: 25,
                    role: "Buffs its allies with random stats (Strength/Regen/Counter/Block) 70% of the time and attacks the rest. "
                        + "Kill it early before its team snowballs."));
                pool.Add(Build("Knight", EnemyArchetype.Knight, 55, 8, 12, counterStacks: 3, gold: 30,
                    role: "High block plus Counter: hitting it lets it strike back. Break its shield or use ignore-block damage."));
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
        d.role = template.role;
        d.phaseThreshold = template.phaseThreshold;
        d.phaseStrength = template.phaseStrength;
        d.phaseRegen = template.phaseRegen;
        d.phaseImmortal = template.phaseImmortal;
        d.phaseHeal = template.phaseHeal;
        d.phasePlayerDebuff = template.phasePlayerDebuff;
        d.enrageMultiplier = template.enrageMultiplier;
        d.canSummon = template.canSummon;
        d.summonCount = template.summonCount;
        d.summonThreshold = template.summonThreshold;
        d.summonId = template.summonId;
        d.canSplit = template.canSplit;
        d.splitCount = template.splitCount;
        d.splitHpScale = template.splitHpScale;
        d.splitDmgScale = template.splitDmgScale;
        d.attackOnly = template.attackOnly;
        d.isSummoned = template.isSummoned;
        d.resummonDelayTurns = template.resummonDelayTurns;
        ApplyVisuals(d);

        return d;
    }

    // =========================================================
    // BOSS MINIONS (quái boss triệu hồi)
    // =========================================================

    public static EnemyData BuildMinion(string id, int mapLevel)
    {
        EnemyData template = GetMapPool(mapLevel).Find(e => e.enemyName == id);

        if (template == null)
        {
            for (int m = 1; m <= 4; m++)
            {
                template = GetMapPool(m).Find(e => e.enemyName == id);
                if (template != null) break;
            }
        }

        if (template == null)
        {
            List<EnemyData> pool = GetMapPool(mapLevel);
            if (pool.Count == 0) pool = GetMapPool(1);
            template = pool[Random.Range(0, pool.Count)];
        }

        EnemyData minion = BuildScaled(template, mapLevel);
        minion.maxHealth = Mathf.Max(5, Mathf.RoundToInt(minion.maxHealth * 0.5f));
        minion.goldReward = 0;
        return minion;
    }

    // Slime con tách ra khi Slime chết (không split tiếp, không cho vàng)
    public static EnemyData BuildSplit(EnemyData parent)
    {
        if (parent == null)
            return null;

        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.enemyName = "Small " + parent.enemyName;
        d.artwork = parent.artwork;
        d.animatorController = parent.animatorController;
        d.archetype = parent.archetype;
        d.maxHealth = Mathf.Max(5, Mathf.RoundToInt(parent.maxHealth * Mathf.Clamp(parent.splitHpScale, 0.2f, 0.9f)));
        d.attackDamage = Mathf.Max(1, Mathf.RoundToInt(parent.attackDamage * Mathf.Max(1, parent.splitDmgScale) * 0.5f));
        d.block = Mathf.Max(0, Mathf.RoundToInt(parent.block * 0.5f));
        d.poisonDamage = parent.poisonDamage;
        d.lifesteal = parent.lifesteal;
        d.selfHeal = parent.selfHeal;
        d.regenValue = parent.regenValue;
        d.buffStrength = parent.buffStrength;
        d.weakDamage = parent.weakDamage;
        d.vulnerableDamage = parent.vulnerableDamage;
        d.counterStacks = parent.counterStacks;
        d.goldReward = 0;
        d.isBoss = false;
        d.canSplit = false;
        return d;
    }

    // =========================================================
    // MINI BOSS & BOSS (single source of truth)
    // =========================================================

    public static EnemyData BuildMiniBossCore(int mapLevel)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.isBoss = true;
        d.goldReward = 60 + (mapLevel - 1) * 15;

        switch (mapLevel)
        {
            case 1:
            case 2:
                d.enemyName = "Mini Boss Knight";
                d.archetype = EnemyArchetype.Knight;
                d.maxHealth = 90;
                d.attackDamage = 12;
                d.block = 10;
                d.counterStacks = 3;
                d.selfHeal = 10;
                d.phaseThreshold = 0.4f;
                d.phaseStrength = 2;
                d.phaseHeal = 10;
                d.phasePlayerDebuff = 1;
                d.enrageMultiplier = 2;
                d.role = "A shield-wall boss: high block, Counter and self-heal. Below 40% HP enters Phase 2 "
                    + "(+2 Strength, heals 10, applies Weak + Vulnerable to you) and the next attack is enraged x2. "
                    + "Burst through its shield, don't let it carry block into a new turn.";
                break;

            case 3:
                d.enemyName = "Mini Boss Priest";
                d.archetype = EnemyArchetype.Priest;
                d.maxHealth = 100;
                d.attackDamage = 6;
                d.block = 8;
                d.selfHeal = 20;
                d.regenValue = 4;
                d.buffStrength = 2;
                d.weakDamage = 3;
                d.canSummon = true;
                d.summonCount = 2;
                d.summonThreshold = 0f;
                d.resummonDelayTurns = 2;
                d.phaseThreshold = 0.4f;
                d.phaseRegen = 4;
                d.phaseHeal = 15;
                d.phasePlayerDebuff = 2;
                d.enrageMultiplier = 2;
                d.role = "A support boss: summons minions from earlier maps and buffs them with random stats "
                    + "(Strength/Regen/Counter/Block) while debuffing you on buff turns. "
                    + "Its minions can only attack and rely on its buffs - clear them fast, it resummons 2 more after 2 turns. "
                    + "Below 40% HP Phase 2: +4 Regen, heals 15, Weak + Vulnerable x2, enrage x2.";
                break;

            default:
                d.enemyName = "Mini Boss Assassin";
                d.archetype = EnemyArchetype.Assassin;
                d.maxHealth = 80;
                d.attackDamage = 16;
                d.block = 0;
                d.vulnerableDamage = 3;
                d.phaseThreshold = 0.4f;
                d.phaseStrength = 3;
                d.phasePlayerDebuff = 2;
                d.enrageMultiplier = 2;
                d.role = "A burst boss: very high damage and applies Vulnerable. "
                    + "Below 40% HP Phase 2: +3 Strength, Weak + Vulnerable x2, enrage x2. "
                    + "Its enraged hit can one-shot - always keep block ready for the big hits.";
                break;
        }

        ApplyVisuals(d);
        return d;
    }

    public static EnemyData BuildMiniBoss(int mapLevel)
    {
        return BuildScaled(BuildMiniBossCore(mapLevel), mapLevel);
    }

    public static EnemyData BuildBossCore(int mapLevel)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.isBoss = true;
        d.goldReward = 100 + (mapLevel - 1) * 25;

        switch (mapLevel)
        {
            case 1:
                d.enemyName = "Boss Golem";
                d.archetype = EnemyArchetype.Golem;
                d.maxHealth = 150;
                d.attackDamage = 14;
                d.block = 8;
                d.selfHeal = 20;
                d.regenValue = 6;
                d.phaseThreshold = 0.5f;
                d.phaseStrength = 3;
                d.phaseRegen = 4;
                d.phaseImmortal = 1;
                d.phaseHeal = 15;
                d.phasePlayerDebuff = 2;
                d.enrageMultiplier = 2;
                d.role = "A tank boss with heavy self-heal and Regen. Below 50% HP Phase 2: +3 Strength, +4 Regen, "
                    + "Immortal 1 turn, heals 15, debuffs you, enrage x2. Once Phase 2 starts, finish it as fast as possible.";
                break;

            case 2:
                d.enemyName = "Boss Knight";
                d.archetype = EnemyArchetype.Knight;
                d.maxHealth = 165;
                d.attackDamage = 12;
                d.block = 14;
                d.counterStacks = 3;
                d.selfHeal = 15;
                d.phaseThreshold = 0.5f;
                d.phaseStrength = 3;
                d.phaseImmortal = 1;
                d.phaseHeal = 12;
                d.phasePlayerDebuff = 2;
                d.enrageMultiplier = 2;
                d.role = "A scaling pressure boss: very high block, Counter and self-heal. Below 50% HP Phase 2: "
                    + "+3 Strength, Immortal 1 turn, heals 12, debuffs you, enrage x2. "
                    + "Break its shield at the right moment, don't blindly swing into it.";
                break;

            case 3:
                d.enemyName = "Boss Assassin";
                d.archetype = EnemyArchetype.Assassin;
                d.maxHealth = 135;
                d.attackDamage = 18;
                d.block = 0;
                d.vulnerableDamage = 3;
                d.phaseThreshold = 0.5f;
                d.phaseStrength = 4;
                d.phaseHeal = 15;
                d.phasePlayerDebuff = 3;
                d.enrageMultiplier = 2;
                d.role = "The highest-damage boss: Vulnerable and no block. Below 50% HP Phase 2: +4 Strength, "
                    + "heals 15, Weak + Vulnerable x3, enrage x2. Enrage stacked with Vulnerable is a deadly combo - "
                    + "keep maximum block in the late game.";
                break;

            default:
                d.enemyName = "Boss Golem Overlord";
                d.archetype = EnemyArchetype.Golem;
                d.maxHealth = 220;
                d.attackDamage = 16;
                d.block = 10;
                d.selfHeal = 25;
                d.regenValue = 6;
                d.phaseThreshold = 0.5f;
                d.phaseStrength = 5;
                d.phaseRegen = 6;
                d.phaseImmortal = 1;
                d.phaseHeal = 30;
                d.phasePlayerDebuff = 3;
                d.enrageMultiplier = 2;
                d.canSummon = true;
                d.summonCount = 2;
                d.summonThreshold = 0.5f;
                d.summonId = "Golem";
                d.role = "The final boss. Below 50% HP it Summons 2 Golems, then Phase 2: +5 Strength, +6 Regen, "
                    + "Immortal 1 turn, heals 30, heavy debuffs, enrage x2. "
                    + "Manage the summoned adds while bursting the boss down.";
                break;
        }

        ApplyVisuals(d);
        return d;
    }

    public static EnemyData BuildBoss(int mapLevel)
    {
        return BuildScaled(BuildBossCore(mapLevel), mapLevel);
    }

    // =========================================================
    // PRIEST MINIONS (Mini Boss Priest triệu hồi, ngẫu nhiên map 1-2)
    // =========================================================

    public static EnemyData BuildPriestMinion()
    {
        List<EnemyData> pool = new();
        foreach (EnemyData e in GetMapPool(1))
            if (e != null && e.archetype != EnemyArchetype.Priest) pool.Add(e);
        foreach (EnemyData e in GetMapPool(2))
            if (e != null && e.archetype != EnemyArchetype.Priest) pool.Add(e);

        if (pool.Count == 0)
            pool.AddRange(GetMapPool(1));

        EnemyData template = pool[Random.Range(0, pool.Count)];

        EnemyData minion = ScriptableObject.CreateInstance<EnemyData>();
        minion.enemyName = template.enemyName;
        minion.artwork = template.artwork;
        minion.animatorController = template.animatorController;
        minion.archetype = EnemyArchetype.Basic;
        minion.maxHealth = template.maxHealth;
        minion.attackDamage = template.attackDamage;
        minion.goldReward = 0;
        minion.attackOnly = true;
        minion.isSummoned = true;
        return minion;
    }

    // =========================================================
    // MIMIC (chest ambush)
    // =========================================================

    public static EnemyData BuildMimic(int mapLevel)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.enemyName = "Mimic";
        d.archetype = EnemyArchetype.Knight;
        d.maxHealth = 60;
        d.attackDamage = 10;
        d.block = 6;
        d.counterStacks = 3;
        d.goldReward = 40;
        d.role = "A mimic disguised as a treasure chest: high HP, block and Counter. "
            + "Every hit you land comes back at you - break its shield or kill it fast.";
        return BuildScaled(d, mapLevel);
    }

    // =========================================================
    // MONSTER CATALOG (dữ liệu cho Monster Index)
    // =========================================================

    public static List<MonsterCatalogEntry> GetMonsterCatalog()
    {
        List<MonsterCatalogEntry> catalog = new();
        Dictionary<string, MonsterCatalogEntry> byName = new();

        void AddOrMerge(EnemyData e, MonsterCategory category, int mapLevel)
        {
            if (e == null || string.IsNullOrEmpty(e.enemyName)) return;

            if (!byName.TryGetValue(e.enemyName, out MonsterCatalogEntry entry))
            {
                entry = new MonsterCatalogEntry { data = e, category = category };
                byName[e.enemyName] = entry;
                catalog.Add(entry);
            }

            if (!entry.maps.Contains(mapLevel))
                entry.maps.Add(mapLevel);
        }

        for (int m = 1; m <= 4; m++)
        {
            foreach (EnemyData e in GetMapPool(m))
                AddOrMerge(e, MonsterCategory.Normal, m);
        }

        for (int m = 1; m <= 4; m++)
            AddOrMerge(BuildMiniBossCore(m), MonsterCategory.MiniBoss, m);

        for (int m = 1; m <= 4; m++)
            AddOrMerge(BuildMimic(m), MonsterCategory.MiniBoss, m);

        for (int m = 1; m <= 4; m++)
            AddOrMerge(BuildBossCore(m), MonsterCategory.Boss, m);

        return catalog;
    }
}
