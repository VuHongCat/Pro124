using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private List<EnemyData> battleSequence;
    private bool battleEnded;
    private GameObject rewardPanel;
    private bool rewardChosen;

    private void Start()
    {
        playerStatus = FindAnyObjectByType<PlayerStatus>();
        playerHealth.OnDamageTaken += OnPlayerDamageTaken;
        playerHealth.OnPlayerDeath += OnPlayerDeath;
        battleSequence = GetBattleSequence();
        RelicManager.EmitBattleStart();
        SpawnNextEnemy();
    }

    private List<EnemyData> GetBattleSequence()
    {
        if (RunSession.IsBossBattle && RunSession.BossSequence != null && RunSession.BossSequence.Count > 0)
            return RunSession.BossSequence;
        if (enemySequence != null && enemySequence.Count > 0)
            return enemySequence;
        Debug.LogWarning("enemySequence empty - using default enemy sequence");
        return RuntimeEnemyLibrary.GetDefaultSequence();
    }

    private void OnPlayerDeath()
    {
        LoseBattle();
    }

    private void SpawnNextEnemy()
    {
        enemyIndex++;
        if (enemyIndex >= battleSequence.Count)
        {
            WinBattle();
            return;
        }
        SpawnEnemy(battleSequence[enemyIndex]);
    }

    private void SpawnEnemy(EnemyData data)
    {
        for (int i = enemyArea.childCount - 1; i >= 0; i--)
            Destroy(enemyArea.GetChild(i).gameObject);

        GameObject enemy = enemyFactory.CreateEnemy(data, enemyArea);
        enemyCombat = enemy.GetComponent<EnemyCombat>();
        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyStatus = enemy.GetComponent<EnemyStatus>();
        enemyHealth.OnEnemyDeath += OnEnemyDeath;
        enemyHealth.OnDamaged += OnEnemyDamaged;
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
        enemy.OnEnemyDeath -= OnEnemyDeath;

        Animator animator = enemy.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            StartCoroutine(FinishEnemyDeath(enemy.gameObject));
        }
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
    public void OnEnemyAnimationFinished(GameObject enemy)
    {
        Destroy(enemy);

        SpawnNextEnemy();
    }

    public void StartPlayerTurn()
    {
        playerBlock.ResetBlock();
        playerStatus = FindAnyObjectByType<PlayerStatus>();
        playerStatus?.OnTurnEnd();
    }

    private void WinBattle()
    {
        if (battleEnded) return;
        battleEnded = true;
        Debug.Log("=== Battle Won! ===");
        GameEvents.OnBattleEnd?.Invoke();
        RelicManager.EmitBattleEnd();
        BuildRewardPanel(RollRewardChoices(3));
    }

    private List<CardData> RollRewardChoices(int count)
    {
        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        List<CardData> result = new();
        List<CardData> pool = db != null ? db.GetComplexCards() : RuntimeCardLibrary.GetCards();
        if (pool.Count == 0)
        {
            CardData c = db != null ? db.GetRandomCard() : RuntimeCardLibrary.GetRandomCard();
            if (c != null) pool.Add(c);
        }

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            CardData card = pool[Random.Range(0, pool.Count)];
            pool.Remove(card);
            result.Add(card);
        }
        return result;
    }

    private void BuildRewardPanel(List<CardData> choices)
    {
        Canvas canvas = RuntimeUi.CreateCanvas("RewardCanvas");
        rewardPanel = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        RuntimeUi.CreateText(rewardPanel.transform, "Victory! Choose 1 card reward", 28, TextAnchor.MiddleCenter,
            new Vector2(0, 0.85f), new Vector2(1, 1));

        for (int i = 0; i < choices.Count; i++)
        {
            CardData card = choices[i];
            RuntimeUi.CreateButton(rewardPanel.transform, CardRewardLabel(card),
                new Vector2(0, 140 - i * 130),
                new Vector2(460, 115),
                () => ChooseReward(card));
        }

        RuntimeUi.CreateButton(rewardPanel.transform, "Skip", new Vector2(0, -320), new Vector2(180, 50), FinishBattle);
    }

    private string CardRewardLabel(CardData c)
    {
        string s = $"{c.cardName} - {c.description}";
        List<string> stats = new();
        if (c.energyCost > 0) stats.Add($"Energy {c.energyCost}");
        if (c.damage > 0) stats.Add($"ATK {c.damage}");
        if (c.block > 0) stats.Add($"Block {c.block}");
        if (c.heal > 0) stats.Add($"Heal {c.heal}");
        if (c.strength > 0) stats.Add($"Str {c.strength}");
        if (stats.Count > 0) s += "\n" + string.Join(" | ", stats);
        return s;
    }

    private void ChooseReward(CardData card)
    {
        if (rewardChosen || rewardPanel == null) return;
        rewardChosen = true;
        deckManager.AddCardToDeck(card);

        foreach (Transform child in rewardPanel.transform)
            Destroy(child.gameObject);

        RuntimeUi.CreateText(rewardPanel.transform, $"Added to deck: {card.cardName}", 24, TextAnchor.MiddleCenter,
            new Vector2(0, 0.55f), new Vector2(1, 0.7f));
        RuntimeUi.CreateButton(rewardPanel.transform, "Continue", new Vector2(0, -200), new Vector2(240, 60), FinishBattle);
    }

    private void FinishBattle()
    {
        RunSession.IsBossBattle = false;
        RunSession.ReturnToMap();
    }

    private void LoseBattle()
    {
        if (battleEnded) return;
        battleEnded = true;
        Debug.Log("=== Player Died ===");
        GameEvents.OnBattleEnd?.Invoke();

        Canvas canvas = RuntimeUi.CreateCanvas("GameOverCanvas");
        GameObject panel = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.9f));
        RuntimeUi.CreateText(panel.transform, "You died...", 34, TextAnchor.MiddleCenter,
            new Vector2(0, 0.6f), new Vector2(1, 0.8f));
        RuntimeUi.CreateButton(panel.transform, "Start new run", new Vector2(0, 0), new Vector2(260, 60), () =>
        {
            RunSession.StartNewRun();
            RunSession.ReturnToMap();
        });
    }
    private IEnumerator FinishEnemyDeath(GameObject enemy)
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(enemy);

        SpawnNextEnemy();
    }
}
