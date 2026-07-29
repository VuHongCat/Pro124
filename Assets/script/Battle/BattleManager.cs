using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Map")]
    [SerializeField] private string returnMapScene = "MapLevel1";

    private readonly List<EnemyHealth> enemies = new();
    private readonly List<EnemyCombat> enemyCombats = new();

    private const string BattleNodeKey = "BattleNode";
    private const string CompletedNodeKey = "CompletedMapNode";

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        SpawnEnemies();
    }

    // =========================================================
    // SPAWN ENEMIES
    // =========================================================

    private void SpawnEnemies()
    {
        enemies.Clear();
        enemyCombats.Clear();

        // Random 1 - 3
        int enemyCount =
            Random.Range(1, 4);

        Debug.Log(
            "Spawn enemy count: "
            + enemyCount
        );

        // Đảm bảo không vượt quá số spawn point
        enemyCount =
            Mathf.Min(
                enemyCount,
                enemySpawnPoints.Length
            );

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy =
                enemyFactory.CreateEnemy(
                    slime,
                    enemyArea
                );

            // -----------------------------------------
            // Đặt vị trí enemy
            // -----------------------------------------

            RectTransform enemyRect =
                enemy.GetComponent<RectTransform>();

            RectTransform spawnRect =
                enemySpawnPoints[i]
                    .GetComponent<RectTransform>();

            if (enemyRect != null &&
                spawnRect != null)
            {
                enemyRect.anchoredPosition =
                    spawnRect.anchoredPosition;
            }
            else
            {
                enemy.transform.position =
                    enemySpawnPoints[i].position;
            }

            // -----------------------------------------
            // Get components
            // -----------------------------------------

            EnemyHealth health =
                enemy.GetComponent<EnemyHealth>();

            EnemyCombat combat =
                enemy.GetComponent<EnemyCombat>();

            // -----------------------------------------
            // Add list
            // -----------------------------------------

            enemies.Add(health);
            enemyCombats.Add(combat);

            // -----------------------------------------
            // Death event
            // -----------------------------------------

            health.OnEnemyDeath += OnEnemyDeath;

            // -----------------------------------------
            // Intent
            // -----------------------------------------

            combat.DecideNextIntent();
        }
    }

    // =========================================================
    // PLAY CARD
    // =========================================================

    public void PlayCard(CardDisplay card)
    {
        if (card == null)
            return;

        if (card.CardData == null)
            return;

        // -----------------------------------------
        // Check Energy
        // -----------------------------------------

        if (!energyManager.HasEnoughEnergy(
                card.CardData.energyCost))
        {
            Debug.Log(
                "Not enough energy!"
            );

            return;
        }

        // -----------------------------------------
        // Attack phải target enemy
        // -----------------------------------------

        if (card.CardData.cardType ==
            CardType.Attack)
        {
            if (EnemyTargetManager.Instance == null)
            {
                Debug.LogError(
                    "EnemyTargetManager chưa tồn tại!"
                );

                return;
            }

            if (!EnemyTargetManager.Instance.HasTarget())
            {
                Debug.Log(
                    "Hãy chọn enemy trước!"
                );

                return;
            }
        }

        // -----------------------------------------
        // Target
        // -----------------------------------------

        EnemyHealth target = null;

        if (card.CardData.cardType ==
            CardType.Attack)
        {
            target =
                EnemyTargetManager.Instance
                    .CurrentTarget;

            if (target == null ||
                target.CurrentHealth <= 0)
            {
                Debug.Log(
                    "Enemy target không còn tồn tại!"
                );

                EnemyTargetManager.Instance
                    .ClearTarget();

                return;
            }
        }

        // -----------------------------------------
        // Spend energy
        // -----------------------------------------

        energyManager.SpendEnergy(
            card.CardData.energyCost
        );

        // -----------------------------------------
        // Resolve card
        // -----------------------------------------

        effectResolver.Resolve(
            card.CardData,
            target
        );

        // -----------------------------------------
        // Discard
        // -----------------------------------------

        deckManager.AddToDiscard(
            card.CardData
        );

        // -----------------------------------------
        // Remove hand
        // -----------------------------------------

        handManager.RemoveCard(card);

        // -----------------------------------------
        // Clear target if dead
        // -----------------------------------------

        if (target != null &&
            target.CurrentHealth <= 0)
        {
            if (EnemyTargetManager.Instance != null)
            {
                EnemyTargetManager.Instance
                    .ClearTarget();
            }
        }
    }

    // =========================================================
    // ENEMY DEATH
    // =========================================================

    private void OnEnemyDeath(
        EnemyHealth enemy)
    {
        enemies.Remove(enemy);

        EnemyCombat combat =
            enemy.GetComponent<EnemyCombat>();

        if (combat != null)
        {
            enemyCombats.Remove(combat);
        }

        // Clear target
        if (EnemyTargetManager.Instance != null)
        {
            if (EnemyTargetManager.Instance
                    .CurrentTarget == enemy)
            {
                EnemyTargetManager.Instance
                    .ClearTarget();
            }
        }

        Debug.Log(
            "Enemy died. Remaining enemies: "
            + enemies.Count
        );

        // -----------------------------------------
        // All enemies dead
        // -----------------------------------------

        if (enemies.Count == 0)
        {
            BattleWon();
        }
    }

    // =========================================================
    // BATTLE WON
    // =========================================================

    private void BattleWon()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "BATTLE WON!"
        );

        Debug.Log(
            "================================"
        );

        // -----------------------------------------
        // Lấy node Battle đã vào
        // -----------------------------------------

        string battleNode =
            PlayerPrefs.GetString(
                BattleNodeKey,
                ""
            );

        if (!string.IsNullOrEmpty(battleNode))
        {
            // Lưu node đã hoàn thành
            PlayerPrefs.SetString(
                CompletedNodeKey,
                battleNode
            );

            PlayerPrefs.Save();

            Debug.Log(
                "Completed Map Node: "
                + battleNode
            );
        }
        else
        {
            Debug.LogWarning(
                "Không tìm thấy BattleNode!"
            );
        }

        // -----------------------------------------
        // Quay lại map
        // -----------------------------------------

        Debug.Log(
            "Return to: "
            + returnMapScene
        );

        SceneManager.LoadScene(
            returnMapScene
        );
    }

    // =========================================================
    // ENEMY TURN
    // =========================================================

    public void EnemyAttack()
    {
        for (
            int i = enemyCombats.Count - 1;
            i >= 0;
            i--
        )
        {
            EnemyCombat combat =
                enemyCombats[i];

            if (combat == null)
                continue;

            combat.ExecuteIntent(
                playerHealth
            );
        }
    }

    // =========================================================
    // PLAYER TURN
    // =========================================================

    public void StartPlayerTurn()
    {
        playerBlock.ResetBlock();
    }
}