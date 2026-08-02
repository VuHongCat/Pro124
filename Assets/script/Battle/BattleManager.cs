using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private CardDatabase database;
    [SerializeField] private HandManager handManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private List<EnemyData> enemySequence = new();
    [SerializeField] private Transform enemyArea;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private CardEffectResolver effectResolver;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;
    private EnemyCombat enemyCombat;
    private EnemyStatus enemyStatus;
    private PlayerStatus playerStatus;
    private int enemyIndex = -1;

    private void Start()
    {
        playerStatus = FindAnyObjectByType<PlayerStatus>();
        if (playerHealth != null)
            playerHealth.OnDamageTaken += OnPlayerDamageTaken;
        else
            Debug.LogWarning("BattleManager.Start: playerHealth is null — OnDamageTaken not subscribed.");

        SpawnNextEnemy();
    }

    private void OnDisable()
    {
        // Unsubscribe from player events
        if (playerHealth != null)
            playerHealth.OnDamageTaken -= OnPlayerDamageTaken;

        // Unsubscribe from current enemy events (if any)
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath -= OnEnemyDeath;
            enemyHealth.OnDamaged -= OnEnemyDamaged;
        }
    }

    private void SpawnNextEnemy()
    {
        enemyIndex++;
        if (enemyIndex >= enemySequence.Count)
        {
            Debug.Log("=== Battle Won! ===");
            return;
        }
        SpawnEnemy(enemySequence[enemyIndex]);
    }

    private void SpawnEnemy(EnemyData data)
    {
        // Clean up any previous enemy subscriptions before destroying/creating
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath -= OnEnemyDeath;
            enemyHealth.OnDamaged -= OnEnemyDamaged;
        }

        for (int i = enemyArea.childCount - 1; i >= 0; i--)
            Destroy(enemyArea.GetChild(i).gameObject);

        GameObject enemy = enemyFactory.CreateEnemy(data, enemyArea);
        if (enemy == null)
        {
            Debug.LogError("SpawnEnemy: enemyFactory returned null");
            return;
        }

        enemyCombat = enemy.GetComponent<EnemyCombat>();
        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyStatus = enemy.GetComponent<EnemyStatus>();

        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDeath += OnEnemyDeath;
            enemyHealth.OnDamaged += OnEnemyDamaged;
        }
        else
        {
            Debug.LogWarning("SpawnEnemy: spawned enemy has no EnemyHealth component");
        }

        if (enemyCombat != null)
            enemyCombat.DecideNextIntent();
    }

    private void OnPlayerDamageTaken(int damage)
    {
        if (playerStatus == null || enemyHealth == null) return;
        int counter = playerStatus.GetStatus(StatusType.Counter);
        if (counter <= 0) return;

        int reflect = Mathf.RoundToInt(damage * 0.6f);
        if (reflect > 0)
            enemyHealth.TakeDamage(reflect, false);
        playerStatus.AddStatus(StatusType.Counter, -1);
    }

    private void OnEnemyDamaged(int damage)
    {
        if (enemyStatus == null || playerHealth == null) return;
        int counter = enemyStatus.GetStatus(StatusType.Counter);
        if (counter <= 0) return;

        int reflect = Mathf.RoundToInt(damage * 0.5f);
        if (reflect > 0)
            playerHealth.TakeDamage(reflect, false);
        enemyStatus.AddStatus(StatusType.Counter, -1);
    }

    public void PlayCard(CardDisplay card)
    {
        if (!energyManager.HasEnoughEnergy(card.CardData.energyCost)) return;
        energyManager.SpendEnergy(card.CardData.energyCost);
        effectResolver.Resolve(card.CardData, enemyHealth);
        deckManager.AddToDiscard(card.CardData);
        handManager.RemoveCard(card);
    }

    private void OnEnemyDeath(EnemyHealth enemy)
    {
        // Unsubscribe this enemy before it gets destroyed
        if (enemy != null)
        {
            enemy.OnEnemyDeath -= OnEnemyDeath;
            enemy.OnDamaged -= OnEnemyDamaged;
        }

        Debug.Log($"=== {enemySequence.Count - enemyIndex - 1} quái còn lại ===");
        StartCoroutine(SpawnNextEnemyNextFrame());
    }

    private IEnumerator SpawnNextEnemyNextFrame()
    {
        yield return null;
        SpawnNextEnemy();
    }

    public void EnemyAttack()
    {
        if (enemyCombat == null) return;
        enemyCombat.ExecuteIntent(playerHealth);
        enemyStatus?.OnTurnEnd();
    }

    public void StartPlayerTurn()
    {
        playerBlock.ResetBlock();
        playerStatus = FindAnyObjectByType<PlayerStatus>();
        playerStatus?.OnTurnEnd();
    }
}
