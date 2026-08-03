using System.Collections.Generic;
using UnityEngine;

public static class RuntimeEnemyLibrary
{
    public static List<EnemyData> GetDefaultSequence()
    {
        return new List<EnemyData>
        {
            Build("Slime", EnemyArchetype.Basic, 40, 8, 6),
            Build("Goblin", EnemyArchetype.Poison, 30, 6, 4, poison: 3),
            Build("Bat", EnemyArchetype.Lifesteal, 22, 5, 3, lifesteal: 2)
        };
    }

    public static EnemyData Build(string name, EnemyArchetype archetype, int maxHealth, int attack, int block,
        int poison = 0, int lifesteal = 0)
    {
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.enemyName = name;
        d.archetype = archetype;
        d.maxHealth = maxHealth;
        d.attackDamage = attack;
        d.block = block;
        d.poisonDamage = poison;
        d.lifesteal = lifesteal;
        return d;
    }
}
