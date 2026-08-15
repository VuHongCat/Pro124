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

    [Tooltip("Describes the monster's role / function in battle, shown in the Monster Index")]
    public string role;


    [Header("Animation")]
    public RuntimeAnimatorController animatorController;
    public string attackStateName;


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

    public int bleedDamage;

    public int counterStacks;
    public int goldReward;
    public bool isBoss;
    public bool flipX;

    [Header("Minion Behaviour")]
    public bool attackOnly;
    public bool isSummoned;
    public int resummonDelayTurns = 2;


    [Header("Boss Mechanics")]
    public float phaseThreshold;
    public int phaseStrength;
    public int phaseRegen;
    public int phaseImmortal;
    public int phaseHeal;
    public int phasePlayerDebuff;
    public int enrageMultiplier = 1;
    public bool canSummon;
    public int summonCount = 1;
    public float summonThreshold;
    public string summonId;

    [Header("Split Mechanic")]
    public bool canSplit;
    public int splitCount = 2;
    public float splitHpScale = 0.5f;
    public int splitDmgScale = 1;
}