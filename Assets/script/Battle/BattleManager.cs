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
    [SerializeField] private CardEffectResolver effectResolver;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;
    private PlayerStatus playerStatus;
    private List<EnemyData> battleSequence;
    private readonly List<EnemyHealth> activeEnemies = new();
    private EnemyHealth lastAttacker;
    private bool battleEnded;
    private bool wasFinalBoss;
    private bool rewardChosen;
    private GameObject rewardPanel;

    private void Start()
    {
        playerStatus = FindAnyObjectByType<PlayerStatus>();
        playerHealth.OnDamageTaken += OnPlayerDamageTaken;
        playerHealth.OnPlayerDeath += OnPlayerDeath;
        battleSequence = GetBattleSequence();
        wasFinalBoss = RunSession.IsBossBattle && RunSession.IsFinalBoss;
        RelicManager.EmitBattleStart();
        SpawnEnemies();

        if (BattleStatusUI.Instance == null)
        {
            GameObject uiGo = new GameObject("BattleStatusUI");
            uiGo.AddComponent<BattleStatusUI>();
        }
    }

    private List<EnemyData> GetBattleSequence()
    {
        if (RunSession.IsBossBattle && RunSession.BossSequence != null && RunSession.BossSequence.Count > 0)
            return RunSession.BossSequence;

        List<EnemyData> encounter = RuntimeEnemyLibrary.GetEncounter(RunSession.MapLevel);
        if (encounter != null && encounter.Count > 0)
            return encounter;

        if (enemySequence != null && enemySequence.Count > 0)
            return enemySequence;

        Debug.LogWarning("enemySequence empty - using default enemy sequence");
        return RuntimeEnemyLibrary.GetDefaultSequence();
    }

    private void OnPlayerDeath()
    {
        LoseBattle();
    }

    private void SpawnEnemies()
    {
        activeEnemies.Clear();

        for (int i = enemyArea.childCount - 1; i >= 0; i--)
            Destroy(enemyArea.GetChild(i).gameObject);

        int count = battleSequence != null ? battleSequence.Count : 0;
        for (int i = 0; i < count; i++)
            SpawnEnemy(battleSequence[i], i, count);

        if (EnemyTargetManager.Instance != null)
        {
            if (activeEnemies.Count == 1)
                EnemyTargetManager.Instance.SelectTarget(activeEnemies[0]);
            else
                EnemyTargetManager.Instance.ClearTarget();
        }
    }

    private void SpawnEnemy(EnemyData data, int index, int total)
    {
        GameObject enemy = enemyFactory.CreateEnemy(data, enemyArea);

        RectTransform rt = enemy.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2((index - (total - 1) * 0.5f) * 300f, 0);
        }

        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        EnemyCombat combat = enemy.GetComponent<EnemyCombat>();

        activeEnemies.Add(health);
        RelicManager.Instance?.ApplyBagOfMables(status);
        health.OnEnemyDeath += OnEnemyDeath;
        health.OnDamaged += damage => OnEnemyDamaged(health, damage);
        combat.DecideNextIntent();
    }

    private void OnPlayerDamageTaken(int damage)
    {
        if (playerStatus == null || lastAttacker == null) return;
        int counter = playerStatus.GetStatus(StatusType.Counter);
        if (counter <= 0) return;

        int reflect = Mathf.RoundToInt(damage * 0.6f);
        if (reflect > 0)
            lastAttacker.TakeDamage(reflect, false);
        playerStatus.AddStatus(StatusType.Counter, -1);
    }

    private void OnEnemyDamaged(EnemyHealth enemy, int damage)
    {
        if (playerHealth == null || enemy == null) return;
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        if (status == null) return;
        int counter = status.GetStatus(StatusType.Counter);
        if (counter <= 0) return;

        int reflect = Mathf.RoundToInt(damage * 0.5f);
        if (reflect > 0)
            playerHealth.TakeDamage(reflect, false);
        status.AddStatus(StatusType.Counter, -1);
    }

    private static readonly HashSet<string> TargetCards = new HashSet<string>
    {
        "Strike", "Bash", "HeavyBlade", "Combo", "Chain Hit", "Last Stand", "Sacrifice",
        "Bloodthirst", "Executioner", "Blade Storm", "Steel Skin", "Shockwave",
        "Shatter Armor", "Intimidate", "Hemorrhage"
    };

    public static bool NeedsTarget(CardData card)
    {
        return card != null && TargetCards.Contains(card.cardName.Trim());
    }

    public void PlayCard(CardDisplay card, EnemyHealth droppedOn = null)
    {
        if (card == null || card.CardData == null)
            return;

        EnemyHealth target = ResolveTarget(card.CardData, droppedOn);

        if (NeedsTarget(card.CardData) && target == null)
            return;

        if (!energyManager.HasEnoughEnergy(card.CardData.energyCost))
            return;

        energyManager.SpendEnergy(card.CardData.energyCost);
        effectResolver.Resolve(card.CardData, target);
        deckManager.AddToDiscard(card.CardData);
        handManager.RemoveCard(card);
    }

    private EnemyHealth ResolveTarget(CardData card, EnemyHealth droppedOn)
    {
        if (droppedOn != null && droppedOn.CurrentHealth > 0)
            return droppedOn;

        if (EnemyTargetManager.Instance != null &&
            EnemyTargetManager.Instance.CurrentTarget != null &&
            EnemyTargetManager.Instance.CurrentTarget.CurrentHealth > 0)
            return EnemyTargetManager.Instance.CurrentTarget;

        if (activeEnemies.Count == 1 && activeEnemies[0] != null && activeEnemies[0].CurrentHealth > 0)
            return activeEnemies[0];

        return null;
    }

    private void OnEnemyDeath(EnemyHealth enemy)
    {
        if (enemy != null)
        {
            enemy.OnEnemyDeath -= OnEnemyDeath;
            activeEnemies.Remove(enemy);

            if (EnemyTargetManager.Instance != null &&
                EnemyTargetManager.Instance.CurrentTarget == enemy)
                EnemyTargetManager.Instance.ClearTarget();
        }

        if (activeEnemies.Count == 0)
        {
            WinBattle();
            return;
        }

        Debug.Log($"{activeEnemies.Count} enemies left");
    }

    public void EnemyAttack()
    {
        foreach (EnemyHealth enemy in activeEnemies)
        {
            if (enemy == null || enemy.CurrentHealth <= 0)
                continue;

            lastAttacker = enemy;
            EnemyCombat combat = enemy.GetComponent<EnemyCombat>();
            EnemyStatus status = enemy.GetComponent<EnemyStatus>();

            if (combat != null)
                combat.ExecuteIntent(playerHealth);

            status?.OnTurnEnd();
        }

        lastAttacker = null;
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
        rewardChosen = false;
        Debug.Log("=== Battle Won! ===");
        GameEvents.OnBattleEnd?.Invoke();
        RelicManager.EmitBattleEnd();
        int gold = GrantBattleGold();
        CardData card = RollReward();
        if (card != null)
            deckManager.AddCardToDeck(card);
        ShowRewardAndContinue(gold, card);
    }

    private int GrantBattleGold()
    {
        int total = 0;

        if (battleSequence != null)
        {
            foreach (EnemyData enemy in battleSequence)
            {
                if (enemy != null)
                    total += enemy.goldReward;
            }
        }

        if (total <= 0)
            return 0;

        return RelicManager.Instance.OnGainGold(total);
    }

    private CardData RollReward()
    {
        List<CardData> pool = new();
        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        if (db != null)
            pool.AddRange(db.GetComplexCards());
        else
            pool.AddRange(RuntimeCardLibrary.GetCards());

        if (pool.Count == 0)
        {
            CardData c = db != null ? db.GetRandomCard() : RuntimeCardLibrary.GetRandomCard();
            if (c != null) pool.Add(c);
        }

        if (pool.Count == 0)
            return null;

        bool isBoss = RunSession.IsBossBattle;

        int totalWeight = 0;
        foreach (CardData card in pool)
            totalWeight += GetRarityWeight(card.rarity, isBoss);

        int roll = Random.Range(0, totalWeight);
        int acc = 0;
        foreach (CardData card in pool)
        {
            acc += GetRarityWeight(card.rarity, isBoss);
            if (roll < acc)
                return card;
        }

        return pool[pool.Count - 1];
    }

    private static int GetRarityWeight(CardRarity rarity, bool isBoss)
    {
        switch (rarity)
        {
            case CardRarity.Common: return isBoss ? 6  : 10;
            case CardRarity.Rare:   return isBoss ? 5  : 3;
            case CardRarity.Epic:   return isBoss ? 3  : 1;
            default:                return isBoss ? 6  : 10;
        }
    }

    private void ShowRewardAndContinue(int gold, CardData card)
    {
        Canvas canvas = RuntimeUi.CreateCanvas("RewardCanvas");
        rewardPanel = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        RuntimeUi.CreateText(rewardPanel.transform, "Victory!", 28, TextAnchor.MiddleCenter,
            new Vector2(0, 0.65f), new Vector2(1, 0.75f));

        if (gold > 0)
        {
            RuntimeUi.CreateText(rewardPanel.transform, $"Gold +{gold}", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.52f), new Vector2(1, 0.62f));
        }

        if (card != null)
        {
            RuntimeUi.CreateText(rewardPanel.transform, $"Card gained: {card.cardName}", 22, TextAnchor.MiddleCenter,
                new Vector2(0, 0.4f), new Vector2(1, 0.5f));
        }

        Invoke(nameof(FinishBattle), 1.5f);
    }

    private void FinishBattle()
    {
        RunSession.IsBossBattle = false;

        if (wasFinalBoss)
        {
            RunSession.AdvanceToNextMap();
            return;
        }

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
}
