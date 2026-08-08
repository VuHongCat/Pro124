using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBlock enemyBlock;
    public event Action OnAttackFinished;
    public event Action<EnemyData> OnSummonRequested;
    private EnemyIntent enemyIntent;
    private EnemyStatus enemyStatus;
    private EnemyHealth enemyHealth;
    private PlayerHealth playerHealth;
    private int turnCount;
    private bool defensiveUsed;
    private bool phaseTriggered;
    private bool enragePending;
    private bool rageAttack;
    private bool summonPending;
    private bool summonTriggered;

    public void Initialize(EnemyData data)
    {
        enemyData = data;
        enemyIntent = GetComponent<EnemyIntent>();
        enemyBlock = GetComponent<EnemyBlock>();
        enemyStatus = GetComponent<EnemyStatus>();
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyStatus != null && enemyData.counterStacks > 0)
            enemyStatus.AddStatus(StatusType.Counter, enemyData.counterStacks);

        if (enemyData != null && enemyData.isBoss && enemyHealth != null)
            enemyHealth.OnHealthChanged += OnBossHealthChanged;
    }

    public void SetPlayerHealth(PlayerHealth player)
    {
        playerHealth = player;
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
            enemyHealth.OnHealthChanged -= OnBossHealthChanged;
    }

    // =========================================================
    // BOSS PHASE 2: khi máu chạm ngưỡng -> buff + enrage
    // =========================================================

    private void OnBossHealthChanged(int current, int max)
    {
        if (enemyData == null || !enemyData.isBoss || max <= 0) return;
        if (current <= 0) return;

        float ratio = (float)current / max;

        if (!phaseTriggered && enemyData.phaseThreshold > 0f && ratio <= enemyData.phaseThreshold)
        {
            phaseTriggered = true;
            Debug.Log($"[Boss] {enemyData.enemyName} enters PHASE 2!");

            if (enemyData.phaseStrength > 0)
                enemyStatus?.AddStatus(StatusType.Strength, enemyData.phaseStrength, 99);
            if (enemyData.phaseRegen > 0)
                enemyStatus?.AddStatus(StatusType.Regen, enemyData.phaseRegen, 99);
            if (enemyData.phaseImmortal > 0)
                enemyStatus?.AddStatus(StatusType.Immortal, 1, enemyData.phaseImmortal);
            if (enemyData.phaseHeal > 0)
                enemyHealth?.Heal(enemyData.phaseHeal);

            enragePending = true;
        }

        if (!summonPending && !summonTriggered &&
            enemyData.canSummon && enemyData.summonThreshold > 0f &&
            ratio <= enemyData.summonThreshold)
        {
            summonPending = true;
            summonTriggered = true;
        }
    }

    public void Attack(PlayerHealth player)
    {
        Attack(player, enemyData.attackDamage);
    }

    public void Attack(PlayerHealth player, int baseDamage)
    {
        int damage = baseDamage;
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

        if (enragePending)
        {
            enragePending = false;
            rageAttack = true;
            int mult = Mathf.Max(1, enemyData.enrageMultiplier);
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage * mult);
            return;
        }

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
            enemyStatus?.AddStatus(StatusType.Regen, enemyData.regenValue, 3);
            enemyStatus?.AddStatus(StatusType.Immortal, 1, 1);
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
            enemyStatus?.AddStatus(StatusType.Immortal, 1, 1);
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
            Debug.Log($"{enemyData.enemyName} is stunned, skipping turn.");
            if (enemyIntent != null)
                enemyIntent.SetIntent(EnemyIntentType.Stun, 0);
            DecideNextIntent();
            return;
        }

        switch (enemyIntent.IntentType)
        {
            case EnemyIntentType.Attack:
                Attack(player, enemyIntent.IntentValue);
                break;

            case EnemyIntentType.LifestealAttack:
                Attack(player, enemyIntent.IntentValue);
                enemyHealth?.Heal(enemyData.lifesteal);
                break;

            case EnemyIntentType.Block:
                enemyBlock.AddBlock(enemyIntent.IntentValue);
                break;

            case EnemyIntentType.Poison:
                PlayerStatus ps = player.GetComponent<PlayerStatus>();
                if (ps != null) ps.AddStatus(StatusType.Poison, enemyIntent.IntentValue, 2);
                break;

            case EnemyIntentType.Buff:
                enemyStatus?.AddStatus(StatusType.Strength, enemyIntent.IntentValue, 99);
                break;

            case EnemyIntentType.Debuff:
                PlayerStatus dps = player.GetComponent<PlayerStatus>();
                if (dps != null)
                {
                    if (enemyData.weakDamage > 0) dps.AddStatus(StatusType.Weak, enemyData.weakDamage, 2);
                    if (enemyData.vulnerableDamage > 0) dps.AddStatus(StatusType.Vulnerable, enemyData.vulnerableDamage, 2);
                }
                break;

            case EnemyIntentType.Heal:
                enemyHealth?.Heal(enemyIntent.IntentValue);
                break;
        }

        // Boss triệu hồi quái (xảy ra trong lượt enemy, an toàn để spawn)
        if (summonPending)
        {
            summonPending = false;
            OnSummonRequested?.Invoke(enemyData);
        }

        // Boss phase 2: ra đòn cường hóa + debuff mạnh lên người chơi
        if (rageAttack)
        {
            rageAttack = false;

            if (player != null && enemyData.phasePlayerDebuff > 0)
            {
                PlayerStatus ps = player.GetComponent<PlayerStatus>();
                if (ps != null)
                {
                    ps.AddStatus(StatusType.Weak, enemyData.phasePlayerDebuff, 2);
                    ps.AddStatus(StatusType.Vulnerable, enemyData.phasePlayerDebuff, 2);
                }
            }
        }

        DecideNextIntent();
    }
}
