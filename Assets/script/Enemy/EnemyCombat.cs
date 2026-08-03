using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBlock enemyBlock;
    public event Action OnAttackFinished;
    private EnemyIntent enemyIntent;
    private EnemyStatus enemyStatus;
    private EnemyHealth enemyHealth;
    private int turnCount;
    private bool defensiveUsed;

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        enemyIntent = GetComponent<EnemyIntent>();
        enemyBlock = GetComponent<EnemyBlock>();
        enemyStatus = GetComponent<EnemyStatus>();
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyStatus != null && enemyData.counterStacks > 0)
            enemyStatus.AddStatus(StatusType.Counter, enemyData.counterStacks);
    }

    public void Attack(PlayerHealth player)
    {
        int damage = enemyData.attackDamage;
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Strength) > 0)
            damage += enemyStatus.GetStatus(StatusType.Strength);
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Weak) > 0)
            damage = Mathf.RoundToInt(damage * 0.75f);
        player.TakeDamage(damage);
        OnAttackFinished?.Invoke();
    }

    public void DecideNextIntent()
    {
        turnCount++;
        switch (enemyData.archetype)
        {
            case EnemyArchetype.Poison:     DecidePoison(); break;
            case EnemyArchetype.Lifesteal:  DecideLifesteal(); break;
            case EnemyArchetype.Golem:      DecideGolem(); break;
            case EnemyArchetype.Knight:     DecideKnight(); break;
            case EnemyArchetype.Assassin:   DecideAssassin(); break;
            case EnemyArchetype.Priest:     DecidePriest(); break;
            default:                        DecideBasic(); break;
        }
    }

    private float HpRatio()
    {
        if (enemyHealth == null || enemyHealth.maxHealth <= 0) return 1f;
        return (float)enemyHealth.currentHealth / enemyHealth.maxHealth;
    }

    private void SetIntent(EnemyIntentType type, int value)
    {
        enemyIntent.SetIntent(type, value);
    }

    private bool Roll(int a, int b)
    {
        return UnityEngine.Random.Range(0, a + b) < a;
    }

    private int RollIndex(params int[] weights)
    {
        int total = 0;
        foreach (int w in weights) total += w;
        int roll = UnityEngine.Random.Range(0, total);
        int acc = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            acc += weights[i];
            if (roll < acc) return i;
        }
        return 0;
    }

    private void DecideBasic()
    {
        float ratio = HpRatio();
        int attackWeight = 1;
        int blockWeight = 1;
        if (ratio >= 0.6f) attackWeight = 3;
        else if (ratio >= 0.3f) attackWeight = 2;
        else blockWeight = 3;

        if (Roll(attackWeight, blockWeight))
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else
            SetIntent(EnemyIntentType.Block, enemyData.block);
    }

    private void DecidePoison()
    {
        float ratio = HpRatio();
        int attackWeight = 1;
        int blockWeight = 1;
        int poisonWeight = enemyData.poisonDamage > 0 ? 2 : 0;
        if (ratio >= 0.6f) attackWeight = 3;
        else if (ratio >= 0.3f) attackWeight = 2;
        else blockWeight = 3;

        int idx = RollIndex(attackWeight, blockWeight, poisonWeight);
        if (idx == 0) SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else if (idx == 1) SetIntent(EnemyIntentType.Block, enemyData.block);
        else SetIntent(EnemyIntentType.Poison, enemyData.poisonDamage);
    }

    private void DecideLifesteal()
    {
        float ratio = HpRatio();
        int idx;
        if (ratio >= 0.3f)
            idx = RollIndex(3, 1);
        else
            idx = RollIndex(1, 1, 2);

        if (idx == 0) SetIntent(EnemyIntentType.LifestealAttack, enemyData.attackDamage);
        else if (idx == 1) SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else SetIntent(EnemyIntentType.Block, enemyData.block);
    }

    private void DecideGolem()
    {
        float ratio = HpRatio();
        if (ratio < 0.3f && !defensiveUsed)
        {
            defensiveUsed = true;
            enemyStatus?.AddStatus(StatusType.Regen, enemyData.regenValue);
            enemyStatus?.AddStatus(StatusType.Immortal, 1);
            SetIntent(EnemyIntentType.Heal, enemyData.selfHeal);
            return;
        }

        int a = 2, buff = 2, b = 1;
        if (ratio < 0.3f) { a = 1; buff = 1; b = 2; }
        int idx = RollIndex(a, buff, b);
        if (idx == 0) SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else if (idx == 1) SetIntent(EnemyIntentType.Buff, enemyData.buffStrength);
        else SetIntent(EnemyIntentType.Block, enemyData.block);
    }

    private void DecideKnight()
    {
        float ratio = HpRatio();
        if (ratio < 0.3f && !defensiveUsed)
        {
            defensiveUsed = true;
            enemyStatus?.AddStatus(StatusType.Immortal, 1);
            SetIntent(EnemyIntentType.Block, enemyData.block * 2);
            return;
        }

        int a = 2, b = 1;
        if (ratio < 0.3f) { a = 1; b = 2; }
        if (Roll(a, b))
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else
            SetIntent(EnemyIntentType.Block, enemyData.block);
    }

    private void DecideAssassin()
    {
        float ratio = HpRatio();
        if (ratio < 0.3f)
        {
            if (Roll(4, 1))
                SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
            else
                SetIntent(EnemyIntentType.Debuff, enemyData.vulnerableDamage);
            return;
        }

        if (turnCount % 2 == 0)
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else
            SetIntent(EnemyIntentType.Debuff, enemyData.vulnerableDamage);
    }

    private void DecidePriest()
    {
        float ratio = HpRatio();
        int healW = 2, buffW = 1, debuffW = 1, blkW = 1;
        if (ratio < 0.3f) { healW = 3; buffW = 1; debuffW = 1; blkW = 2; }

        int idx = RollIndex(healW, buffW, debuffW, blkW);
        if (idx == 0) SetIntent(EnemyIntentType.Heal, enemyData.selfHeal);
        else if (idx == 1) SetIntent(EnemyIntentType.Buff, enemyData.buffStrength);
        else if (idx == 2) SetIntent(EnemyIntentType.Debuff, enemyData.weakDamage);
        else SetIntent(EnemyIntentType.Block, enemyData.block);
    }

    public void ExecuteIntent(PlayerHealth player)
    {
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Stun) > 0)
        {
            Debug.Log($"{enemyData.enemyName} bị choáng, bỏ lượt.");
            if (enemyIntent != null)
                enemyIntent.SetIntent(EnemyIntentType.Stun, 0);
            DecideNextIntent();
            return;
        }

        switch (enemyIntent.IntentType)
        {
            case EnemyIntentType.Attack:
                Attack(player);
                break;

            case EnemyIntentType.LifestealAttack:
                Attack(player);
                enemyHealth?.Heal(enemyData.lifesteal);
                break;

            case EnemyIntentType.Block:
                enemyBlock.AddBlock(enemyIntent.IntentValue);
                break;

            case EnemyIntentType.Poison:
                PlayerStatus ps = player.GetComponent<PlayerStatus>();
                if (ps != null) ps.AddStatus(StatusType.Poison, enemyIntent.IntentValue);
                break;

            case EnemyIntentType.Buff:
                enemyStatus?.AddStatus(StatusType.Strength, enemyIntent.IntentValue);
                break;

            case EnemyIntentType.Debuff:
                PlayerStatus dps = player.GetComponent<PlayerStatus>();
                if (dps != null)
                {
                    if (enemyData.weakDamage > 0) dps.AddStatus(StatusType.Weak, enemyData.weakDamage);
                    if (enemyData.vulnerableDamage > 0) dps.AddStatus(StatusType.Vulnerable, enemyData.vulnerableDamage);
                }
                break;

            case EnemyIntentType.Heal:
                enemyHealth?.Heal(enemyIntent.IntentValue);
                break;
        }
        DecideNextIntent();
    }
}
