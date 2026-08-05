using UnityEngine;

public enum EnemyArchetype
{
    Basic,
    Poison,
    Lifesteal,
    Golem,
    Knight,
    Assassin,
    Priest
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName;
    public Sprite artwork;
    public EnemyArchetype archetype;


    [Header("Animation")]
    public RuntimeAnimatorController animatorController;


    [Header("Stats")]
    public int maxHealth;
    public int attackDamage;
    public int block;

    public int poisonDamage;
    public int lifesteal;

    public int selfHeal;
    public int regenValue;

    public int buffStrength;

    public int weakDamage;
    public int vulnerableDamage;

    public int counterStacks;
    public int goldReward;
    public bool isBoss;
    public bool flipX;
}