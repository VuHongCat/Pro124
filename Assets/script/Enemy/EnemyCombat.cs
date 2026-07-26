using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBlock enemyBlock;
    public event Action OnAttackFinished;
    private EnemyIntent enemyIntent;
    private int turnCount;
    public void Initialize(EnemyData data)
    {
        enemyData = data;
        enemyIntent = GetComponent<EnemyIntent>();
        enemyBlock = GetComponent<EnemyBlock>();
    }

    public void Attack(PlayerHealth player)
    {
        player.TakeDamage(enemyData.attackDamage);

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
