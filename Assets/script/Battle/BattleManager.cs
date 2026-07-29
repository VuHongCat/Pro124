using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Card System")]
    [SerializeField] private CardDatabase database;
    [SerializeField] private HandManager handManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private EnergyManager energyManager;
    [SerializeField] private CardEffectResolver effectResolver;

    [Header("Enemy")]
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private EnemyData slime;
    [SerializeField] private Transform enemyArea;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;

    private readonly List<EnemyHealth> enemies = new();
    private readonly List<EnemyCombat> enemyCombats = new();

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        enemies.Clear();
        enemyCombats.Clear();

        // Random 1 - 3 enemy
        int enemyCount = Random.Range(1, 4);

        Debug.Log("Spawn enemy count: " + enemyCount);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy =
                enemyFactory.CreateEnemy(slime, enemyArea);

            RectTransform enemyRect =
                enemy.GetComponent<RectTransform>();

            RectTransform spawnRect =
                enemySpawnPoints[i].GetComponent<RectTransform>();

            if (enemyRect != null && spawnRect != null)
            {
                enemyRect.anchoredPosition =
                    spawnRect.anchoredPosition;
            }
            else
            {
                enemy.transform.position =
                    enemySpawnPoints[i].position;
            }

            EnemyHealth health =
                enemy.GetComponent<EnemyHealth>();

            EnemyCombat combat =
                enemy.GetComponent<EnemyCombat>();

            enemies.Add(health);
            enemyCombats.Add(combat);

            health.OnEnemyDeath += OnEnemyDeath;

            // Mỗi enemy tự quyết định Intent
            combat.DecideNextIntent();
        }
    }

    public void PlayCard(CardDisplay card)
    {
        if (card == null)
            return;

        // Kiểm tra Energy
        if (!energyManager.HasEnoughEnergy(
                card.CardData.energyCost))
        {
            Debug.Log("Not enough energy!");
            return;
        }

        // Nếu là Attack thì phải chọn enemy
        if (card.CardData.cardType == CardType.Attack)
        {
            if (EnemyTargetManager.Instance == null)
            {
                Debug.LogError("EnemyTargetManager chưa tồn tại!");
                return;
            }

            if (!EnemyTargetManager.Instance.HasTarget())
            {
                Debug.Log("Hãy chọn enemy trước!");
                return;
            }
        }

        // Trừ Energy
        energyManager.SpendEnergy(
            card.CardData.energyCost);

        // Lấy target đã click
        EnemyHealth target = null;

        if (card.CardData.cardType == CardType.Attack)
        {
            target =
                EnemyTargetManager.Instance.CurrentTarget;

            // Kiểm tra target còn sống
            if (target == null ||
                target.CurrentHealth <= 0)
            {
                Debug.Log("Enemy target không còn tồn tại!");

                EnemyTargetManager.Instance.ClearTarget();
                return;
            }
        }

        // Thực hiện hiệu ứng card
        effectResolver.Resolve(
            card.CardData,
            target);

        // Đưa card vào discard
        deckManager.AddToDiscard(
            card.CardData);

        // Xóa card khỏi hand
        handManager.RemoveCard(card);

        // Nếu target đã chết
        if (target != null &&
            target.CurrentHealth <= 0)
        {
            EnemyTargetManager.Instance.ClearTarget();
        }
    }

    private void OnEnemyDeath(EnemyHealth enemy)
    {
        enemies.Remove(enemy);

        EnemyCombat combat =
            enemy.GetComponent<EnemyCombat>();

        if (combat != null)
            enemyCombats.Remove(combat);

        // Nếu enemy chết đang được chọn
        if (EnemyTargetManager.Instance != null &&
            EnemyTargetManager.Instance.CurrentTarget == enemy)
        {
            EnemyTargetManager.Instance.ClearTarget();
        }

        Debug.Log(
            "Enemy died. Remaining enemies: "
            + enemies.Count);

        // Không còn enemy
        if (enemies.Count == 0)
        {
            BattleWon();
        }
    }

    private void BattleWon()
    {
        Debug.Log("BATTLE WON!");

        // Sau này chuyển sang Map
        // SceneLoader.Instance.LoadScene("MapLevel1");
    }

    public void EnemyAttack()
    {
        // Cho tất cả enemy thực hiện Intent
        for (int i = enemyCombats.Count - 1; i >= 0; i--)
        {
            EnemyCombat combat = enemyCombats[i];

            if (combat == null)
                continue;

            combat.ExecuteIntent(playerHealth);
        }
    }

    public void StartPlayerTurn()
    {
        playerBlock.ResetBlock();
    }
}