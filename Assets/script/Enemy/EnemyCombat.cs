using System.Collections.Generic;
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
    private bool isPriestBoss;
    private int resummonTurns = -1;
    private int buffKind;
    private int buffAmount;
    private EnemyStatus buffTarget;
    private EnemyIntentType lastIntentType;
    private int sameTypeCount;

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

        isPriestBoss = enemyData != null && enemyData.isBoss && enemyData.archetype == EnemyArchetype.Priest;
        if (isPriestBoss)
            resummonTurns = 0;
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
    // BOSS PHASE 2: when health hits the threshold -> buff + enrage
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

        EnemyDisplay display = GetComponent<EnemyDisplay>();
        if (display != null)
        {
            EnemyHealth self = GetComponent<EnemyHealth>();
            display.Lunge(() =>
            {
                if (self != null)
                    FindAnyObjectByType<BattleManager>()?.OnEnemyAttackHit(self);
                player.TakeDamage(damage);
            }, () => OnAttackFinished?.Invoke());
        }
        else
        {
            player.TakeDamage(damage);
            OnAttackFinished?.Invoke();
        }
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

        // Enemy can only attack (minion of Mini Boss Priest)
        if (enemyData.attackOnly)
        {
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
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
        if (type == lastIntentType)
            sameTypeCount++;
        else
        {
            lastIntentType = type;
            sameTypeCount = 1;
        }
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

    private bool IsBlocked(EnemyIntentType type)
    {
        return type == lastIntentType && sameTypeCount >= 2;
    }

    private int WeightIfAvailable(int weight, EnemyIntentType type)
    {
        return weight > 0 && !IsBlocked(type) ? weight : 0;
    }

    private void DecideBasic()
    {
        float ratio = HpRatio();
        int attackWeight = 1;
        int blockWeight = 1;
        if (ratio >= 0.6f) attackWeight = 3;
        else if (ratio >= 0.3f) attackWeight = 2;
        else blockWeight = 3;

        if (Roll(WeightIfAvailable(attackWeight, EnemyIntentType.Attack),
                 WeightIfAvailable(blockWeight, EnemyIntentType.Block)))
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

        int idx = RollIndex(WeightIfAvailable(attackWeight, EnemyIntentType.Attack),
                            WeightIfAvailable(blockWeight, EnemyIntentType.Block),
                            WeightIfAvailable(poisonWeight, EnemyIntentType.Poison));
        if (idx == 0) SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else if (idx == 1) SetIntent(EnemyIntentType.Block, enemyData.block);
        else SetIntent(EnemyIntentType.Poison, enemyData.poisonDamage);
    }

    private void DecideLifesteal()
    {
        float ratio = HpRatio();
        int idx;
        if (ratio >= 0.3f)
            idx = RollIndex(WeightIfAvailable(3, EnemyIntentType.LifestealAttack),
                            WeightIfAvailable(1, EnemyIntentType.Attack));
        else
            idx = RollIndex(WeightIfAvailable(1, EnemyIntentType.LifestealAttack),
                            WeightIfAvailable(1, EnemyIntentType.Attack),
                            WeightIfAvailable(2, EnemyIntentType.Block));

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
        int idx = RollIndex(WeightIfAvailable(a, EnemyIntentType.Attack),
                            WeightIfAvailable(buff, EnemyIntentType.Buff),
                            WeightIfAvailable(b, EnemyIntentType.Block));
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
        if (Roll(WeightIfAvailable(a, EnemyIntentType.Attack),
                 WeightIfAvailable(b, EnemyIntentType.Block)))
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else
            SetIntent(EnemyIntentType.Block, enemyData.block);
    }

    private void DecideAssassin()
    {
        float ratio = HpRatio();
        if (ratio < 0.3f)
        {
            int a = WeightIfAvailable(4, EnemyIntentType.Attack);
            int d = WeightIfAvailable(1, EnemyIntentType.Debuff);
            if (a + d == 0)
            {
                SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
                return;
            }
            if (Roll(a, d))
                SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
            else
                SetIntent(EnemyIntentType.Debuff, enemyData.vulnerableDamage);
            return;
        }

        if (IsBlocked(EnemyIntentType.Attack))
        {
            SetIntent(EnemyIntentType.Debuff, enemyData.vulnerableDamage);
            return;
        }
        if (IsBlocked(EnemyIntentType.Debuff))
        {
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
            return;
        }

        if (turnCount % 2 == 0)
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
        else
            SetIntent(EnemyIntentType.Debuff, enemyData.vulnerableDamage);
    }

    private void DecidePriest()
    {
        bool attackBlocked = IsBlocked(EnemyIntentType.Attack);
        bool buffBlocked = IsBlocked(EnemyIntentType.Buff);

        if (attackBlocked && buffBlocked)
        {
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
            return;
        }
        if (attackBlocked)
        {
            SetBuffIntent();
            return;
        }
        if (buffBlocked)
        {
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
            return;
        }

        // 30% attack / 70% buff (buff random stats for allies)
        if (Roll(3, 7))
        {
            SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
            return;
        }

        SetBuffIntent();
    }

    private void SetBuffIntent()
    {
        buffKind = UnityEngine.Random.Range(0, 4);
        buffAmount = RandomBuffAmount();
        buffTarget = GetBuffTarget();
        SetIntent(EnemyIntentType.Buff, buffAmount);
    }

    private int RandomBuffAmount()
    {
        switch (buffKind)
        {
            case 0: return UnityEngine.Random.Range(2, 4); // Strength 2-3
            case 1: return UnityEngine.Random.Range(2, 4); // Regen 2-3
            case 2: return UnityEngine.Random.Range(2, 4); // Counter 2-3
            default: return enemyData.block > 0 ? enemyData.block : UnityEngine.Random.Range(6, 11);
        }
    }

    private EnemyStatus GetBuffTarget()
    {
        EnemyHealth[] all = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        List<EnemyHealth> allies = new();
        foreach (EnemyHealth h in all)
        {
            if (h == null || h == enemyHealth) continue;
            if (h.CurrentHealth > 0)
                allies.Add(h);
        }
        if (allies.Count > 0)
            return allies[UnityEngine.Random.Range(0, allies.Count)].GetComponent<EnemyStatus>();
        return enemyStatus;
    }

    private void ApplyBuff(EnemyStatus target)
    {
        if (target == null) return;

        switch (buffKind)
        {
            case 1: target.AddStatus(StatusType.Regen, buffAmount, 3); break;
            case 2: target.AddStatus(StatusType.Counter, buffAmount, 99); break;
            case 3:
                EnemyBlock blk = target.GetComponent<EnemyBlock>();
                if (blk != null) blk.AddBlock(buffAmount);
                break;
            default: target.AddStatus(StatusType.Strength, buffAmount, 99); break;
        }
    }

    // Mini Boss Priest: when both minions die, wait resummonDelayTurns turns then summon 2 new ones
    private void UpdatePriestSummon()
    {
        int aliveMinions = CountAliveMinions();

        if (resummonTurns == 0)
        {
            if (aliveMinions == 0)
            {
                summonPending = true;
                resummonTurns = -1;
            }
            return;
        }

        if (resummonTurns > 0)
        {
            resummonTurns--;
            if (resummonTurns == 0)
            {
                summonPending = true;
                resummonTurns = -1;
            }
            return;
        }

        if (aliveMinions == 0)
            resummonTurns = enemyData != null ? enemyData.resummonDelayTurns : 2;
    }

    private int CountAliveMinions()
    {
        EnemyHealth[] all = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
        int count = 0;
        foreach (EnemyHealth h in all)
        {
            if (h == null || h.Data == null) continue;
            if (h.Data.isSummoned && h.CurrentHealth > 0)
                count++;
        }
        return count;
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

        if (isPriestBoss)
            UpdatePriestSummon();

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
                if (buffTarget != null)
                {
                    EnemyHealth targetHp = buffTarget.GetComponent<EnemyHealth>();
                    if (targetHp == null || targetHp.CurrentHealth <= 0)
                        buffTarget = null;
                }
                if (buffTarget != null)
                {
                    ApplyBuff(buffTarget);
                    buffTarget = null;

                    // Mini Boss Priest: buff turn also debuffs the player
                    if (isPriestBoss && player != null)
                    {
                        PlayerStatus buffDps = player.GetComponent<PlayerStatus>();
                        if (buffDps != null)
                        {
                            if (enemyData.weakDamage > 0) buffDps.AddStatus(StatusType.Weak, enemyData.weakDamage, 2);
                            if (enemyData.vulnerableDamage > 0) buffDps.AddStatus(StatusType.Vulnerable, enemyData.vulnerableDamage, 2);
                        }
                    }
                }
                else
                {
                    enemyStatus?.AddStatus(StatusType.Strength, enemyIntent.IntentValue, 99);
                }
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

        // Boss summons enemies (happens during the enemy turn, safe to spawn)
        if (summonPending)
        {
            summonPending = false;
            OnSummonRequested?.Invoke(enemyData);
        }

        // Boss phase 2: unleash an enhanced attack + strong debuff on the player
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
