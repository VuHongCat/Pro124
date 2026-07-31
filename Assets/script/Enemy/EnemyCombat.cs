using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBlock enemyBlock;
    public event Action OnAttackFinished;
    private EnemyIntent enemyIntent;
    private EnemyStatus enemyStatus;
    private int turnCount;
    public void Initialize(EnemyData data)
    {
        enemyData = data;
        enemyIntent = GetComponent<EnemyIntent>();
        enemyBlock = GetComponent<EnemyBlock>();
        enemyStatus = GetComponent<EnemyStatus>();
    }

    public void Attack(PlayerHealth player)
    {
        int damage = enemyData.attackDamage;
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Weak) > 0)
            damage = Mathf.RoundToInt(damage * 0.75f);
        player.TakeDamage(damage);
        OnAttackFinished?.Invoke();
    }

    public void DecideNextIntent()
    {
        turnCount++;
        if (turnCount % 2 == 1)
        {
            enemyIntent.SetIntent(
                EnemyIntentType.Attack,
                enemyData.attackDamage
            );
        }
        else
        {
            enemyIntent.SetIntent(
                EnemyIntentType.Block,
                enemyData.block
            );
        }
    }

    public void ExecuteIntent(PlayerHealth player)
    {
        if (enemyStatus != null && enemyStatus.GetStatus(StatusType.Stun) > 0)
        {
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

            case EnemyIntentType.Block:
                enemyBlock.AddBlock(enemyIntent.IntentValue);
                break;

            case EnemyIntentType.Buff:
                Debug.Log("Enemy Buff");
                break;

            case EnemyIntentType.Debuff:
                Debug.Log("Enemy Debuff");
                break;
        }
        DecideNextIntent();
    }
}
