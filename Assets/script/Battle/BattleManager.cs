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
    [SerializeField] private CardEffectResolver effectResolver;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerBlock playerBlock;
    private PlayerStatus playerStatus;
    private List<EnemyData> battleSequence;
    private readonly List<EnemyHealth> activeEnemies = new();
    private readonly List<EnemyData> pendingSummons = new();
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

            rt.anchoredPosition = new Vector2(
                (index - (total - 1) * 0.5f) * 300f,
                0
            );
        }


        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        EnemyStatus status = enemy.GetComponent<EnemyStatus>();
        EnemyCombat combat = enemy.GetComponent<EnemyCombat>();

        activeEnemies.Add(health);
        RelicManager.Instance?.ApplyBagOfMables(status);
        health.OnEnemyDeath += OnEnemyDeath;
        health.OnDamaged += damage => OnEnemyDamaged(health, damage);
        combat.SetPlayerHealth(playerHealth);
        combat.OnSummonRequested += OnBossSummonRequested;
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

        // Spawn VFX
        if (card.CardData.attackVFX != null && target != null)
        {
            GameObject vfx = Instantiate(card.CardData.attackVFX, target.transform);

            RectTransform enemyRect = target.GetComponent<RectTransform>();
            float offsetX = enemyRect != null
                ? -(enemyRect.rect.width * 0.5f + 70f)
                : -190f;
            vfx.transform.localPosition = new Vector3(offsetX, 0f, 0f);
        }

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

            SpawnSplits(enemy);
        }

        if (activeEnemies.Count == 0)
        {
            WinBattle();
            return;
        }

        Debug.Log($"{activeEnemies.Count} enemies left");
    }

    private void SpawnSplits(EnemyHealth enemy)
    {
        if (enemy == null || enemy.Data == null || !enemy.Data.canSplit)
            return;

        int count = Mathf.Max(1, enemy.Data.splitCount);
        List<EnemyData> splits = new();
        for (int i = 0; i < count; i++)
        {
            EnemyData split = RuntimeEnemyLibrary.BuildSplit(enemy.Data);
            if (split != null)
                splits.Add(split);
        }

        int total = activeEnemies.Count + splits.Count;
        for (int i = 0; i < splits.Count; i++)
            SpawnEnemy(splits[i], activeEnemies.Count + i, total);

        if (splits.Count > 0)
            Debug.Log($"[Split] {enemy.Data.enemyName} split into {splits.Count} smaller enemies.");
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

        ProcessPendingSummons();

        lastAttacker = null;
    }

    private void OnBossSummonRequested(EnemyData boss)
    {
        if (boss == null || string.IsNullOrEmpty(boss.summonId)) return;

        EnemyData minion = RuntimeEnemyLibrary.BuildMinion(boss.summonId, RunSession.MapLevel);
        if (minion == null) return;

        int count = Mathf.Max(1, boss.summonCount);
        for (int i = 0; i < count; i++)
            pendingSummons.Add(minion);
    }

    private void ProcessPendingSummons()
    {
        if (pendingSummons.Count == 0) return;

        int total = pendingSummons.Count;
        for (int i = 0; i < total; i++)
            SpawnEnemy(pendingSummons[i], activeEnemies.Count + i, activeEnemies.Count + total);

        pendingSummons.Clear();
        Debug.Log($"[Boss] Summoned {total} minion(s).");
    }
    public void OnEnemyAnimationFinished(GameObject enemy)
    {
        Destroy(enemy);
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
        List<CardData> rewards = RollRewards();
        ShowRewardAndContinue(gold, rewards);
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

    private List<CardData> RollRewards(int count = 3)
    {
        List<CardData> pool = new();
        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        if (db != null)
            pool.AddRange(db.GetRewardCards());
        else
            pool.AddRange(RuntimeCardLibrary.GetCards());

        if (pool.Count == 0)
        {
            CardData c = db != null ? db.GetRandomCard() : RuntimeCardLibrary.GetRandomCard();
            if (c != null) pool.Add(c);
        }

        if (pool.Count == 0)
            return new List<CardData>();

        bool isBoss = RunSession.IsBossBattle;

        List<CardData> result = new();
        HashSet<string> chosen = new();
        int guard = 0;
        while (result.Count < count && result.Count < pool.Count && guard++ < 200)
        {
            CardData pick = WeightedPick(pool, isBoss, chosen);
            if (pick == null) break;
            chosen.Add(pick.cardName);
            result.Add(pick);
        }

        return result;
    }

    private CardData WeightedPick(List<CardData> pool, bool isBoss, HashSet<string> exclude)
    {
        int totalWeight = 0;
        foreach (CardData card in pool)
        {
            if (exclude.Contains(card.cardName)) continue;
            totalWeight += GetRarityWeight(card.rarity, isBoss);
        }

        if (totalWeight <= 0) return null;

        int roll = Random.Range(0, totalWeight);
        int acc = 0;
        foreach (CardData card in pool)
        {
            if (exclude.Contains(card.cardName)) continue;
            acc += GetRarityWeight(card.rarity, isBoss);
            if (roll < acc) return card;
        }

        return null;
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

    private void ShowRewardAndContinue(int gold, List<CardData> rewards)
    {
        Canvas canvas = RuntimeUi.CreateCanvas("RewardCanvas");
        rewardPanel = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.88f));
        RuntimeUi.CreateText(rewardPanel.transform, "Victory!", 30, TextAnchor.MiddleCenter,
            new Vector2(0, 0.78f), new Vector2(1, 0.88f));

        if (gold > 0)
        {
            RuntimeUi.CreateText(rewardPanel.transform, $"Gold +{gold}", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.68f), new Vector2(1, 0.76f));
        }

        RuntimeUi.CreateText(rewardPanel.transform, "Choose a card", 18, TextAnchor.MiddleCenter,
            new Vector2(0, 0.58f), new Vector2(1, 0.66f));

        int shown = Mathf.Min(rewards.Count, 3);
        for (int i = 0; i < shown; i++)
            CreateCardOption(rewardPanel.transform, rewards[i], i, shown);

        RuntimeUi.CreateText(rewardPanel.transform, "Choose a card to continue", 16, TextAnchor.MiddleCenter,
            new Vector2(0, 0.045f), new Vector2(1, 0.12f));
    }

    private void CreateCardOption(Transform parent, CardData card, int index, int count)
    {
        float cx = count == 3 ? new float[] { 0.22f, 0.5f, 0.78f }[index] : 0.5f;

        GameObject slot = new GameObject("CardOption", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = slot.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(cx - 0.14f, 0.1f);
        rt.anchorMax = new Vector2(cx + 0.14f, 0.56f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image slotImg = slot.GetComponent<Image>();
        slotImg.color = new Color(0.12f, 0.12f, 0.16f, 0.95f);
        Button btn = slot.GetComponent<Button>();
        btn.targetGraphic = slotImg;
        CardData captured = card;
        btn.onClick.AddListener(() => ChooseReward(captured));

        GameObject artGo = new GameObject("Art", typeof(RectTransform), typeof(Image));
        RectTransform artRt = artGo.GetComponent<RectTransform>();
        artRt.SetParent(rt, false);
        artRt.anchorMin = new Vector2(0.08f, 0.3f);
        artRt.anchorMax = new Vector2(0.92f, 0.94f);
        artRt.offsetMin = Vector2.zero;
        artRt.offsetMax = Vector2.zero;
        Image art = artGo.GetComponent<Image>();
        art.preserveAspect = true;
        if (card.artwork != null)
        {
            art.sprite = card.artwork;
            art.color = Color.white;
        }
        else
        {
            art.sprite = GetRewardPlaceholder();
            art.color = new Color(0.1f, 0.1f, 0.12f, 1f);
        }

        Text name = RuntimeUi.CreateText(slot.transform, card.cardName, 18, TextAnchor.MiddleCenter,
            new Vector2(0.04f, 0.2f), new Vector2(0.96f, 0.3f));
        name.fontStyle = FontStyle.Bold;
        name.color = new Color(0.95f, 0.9f, 0.4f);

        Text cost = RuntimeUi.CreateText(slot.transform, card.energyCost.ToString(), 16, TextAnchor.MiddleCenter,
            new Vector2(0.04f, 0.9f), new Vector2(0.2f, 1f));
        cost.color = new Color(1f, 0.85f, 0.3f);

        Text desc = RuntimeUi.CreateText(slot.transform, card.description, 12, TextAnchor.UpperLeft,
            new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.19f));
        desc.color = new Color(0.85f, 0.85f, 0.85f);
    }

    private static Sprite rewardPlaceholder;

    private static Sprite GetRewardPlaceholder()
    {
        if (rewardPlaceholder == null)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            rewardPlaceholder = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            rewardPlaceholder.name = "CardRewardPlaceholder";
        }

        return rewardPlaceholder;
    }

    private void ChooseReward(CardData card)
    {
        if (rewardChosen || card == null) return;
        rewardChosen = true;
        deckManager.AddCardToDeck(Instantiate(card));
        Destroy(rewardPanel);
        FinishBattle();
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
