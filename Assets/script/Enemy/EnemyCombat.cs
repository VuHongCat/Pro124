using UnityEngine;
using System;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    public event Action OnAttackFinished;
    private EnemyIntent enemyIntent;
    public void Initialize(EnemyData data)
    {
        enemyData = data;
        enemyIntent = GetComponent<EnemyIntent>();
    }

    public void Attack(PlayerHealth player)
    {
        player.TakeDamage(enemyData.attackDamage);

        OnAttackFinished?.Invoke();
        DecideNextIntent();
    }

    public void DecideNextIntent()
    {
        enemyIntent.SetIntent(EnemyIntentType.Attack, enemyData.attackDamage);
    }
}
