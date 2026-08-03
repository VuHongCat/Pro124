using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("All Map Nodes")]
    public List<MapNode> nodes = new List<MapNode>();

    [Header("Current Node")]
    public MapNode currentNode;

    [Header("Boss Enemies")]
    public EnemyData miniBossEnemy;
    public EnemyData bossEnemy;

    private GameObject popupRoot;
    private bool popupOpen;

    [Header("Map Scene")]
    [SerializeField] private string mapSceneName = "MapLevel1";

    // =========================================================
    // PLAYER PREFS KEYS
    // =========================================================

    private const string BattleNodeKey = "BattleNode";
    private const string CompletedNodeKey = "CompletedMapNode";

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        GetAllNodes();

        UpdateMapFromProgress();
    }

    // =========================================================
    // GET ALL NODES
    // =========================================================

    public void GetAllNodes()
    {
        nodes.Clear();

        MapNode[] foundNodes =
            FindObjectsByType<MapNode>(
                FindObjectsSortMode.None
            );

        foreach (MapNode node in foundNodes)
        {
            if (node != null)
            {
                nodes.Add(node);
            }
        }

        Debug.Log(
            "[MapManager] Total Nodes = " +
            nodes.Count
        );
    }

    // =========================================================
    // UPDATE MAP
    // =========================================================

    private void UpdateMapFromProgress()
    {
        // -----------------------------------------
        // Nếu vừa chết / chưa có progress
        // -----------------------------------------

        string completedNode =
            PlayerPrefs.GetString(
                CompletedNodeKey,
                ""
            );

        if (string.IsNullOrEmpty(completedNode))
        {
            ResetMapToStart();
            return;
        }

        // -----------------------------------------
        // Reset toàn bộ trước
        // -----------------------------------------

        foreach (MapNode node in nodes)
        {
            if (node != null)
            {
                node.ResetNode();
            }
        }

        // -----------------------------------------
        // Tìm node đã hoàn thành
        // -----------------------------------------

        MapNode completed =
            FindNodeByName(completedNode);

        if (completed == null)
        {
            Debug.LogWarning(
                "[MapManager] Không tìm thấy Completed Node: " +
                completedNode
            );

            ResetMapToStart();
            return;
        }

        // -----------------------------------------
        // Đánh dấu hoàn thành
        // -----------------------------------------

        completed.SetLock(false);
        completed.CompleteNode();

        currentNode = completed;

        Debug.Log(
            "[MapManager] Completed Node = " +
            completed.gameObject.name
        );

        // -----------------------------------------
        // Mở node tiếp theo
        // -----------------------------------------

        UnlockNextNodes(completed);

        // -----------------------------------------
        // Mở line
        // -----------------------------------------

        UnlockLines(completed);

        // -----------------------------------------
        // Xóa progress tạm
        // -----------------------------------------

        PlayerPrefs.DeleteKey(
            CompletedNodeKey
        );

        PlayerPrefs.DeleteKey(
            BattleNodeKey
        );

        PlayerPrefs.Save();
    }

    // =========================================================
    // RESET MAP
    // =========================================================

    public void ResetMapToStart()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "[MapManager] RESET MAP TO START"
        );

        Debug.Log(
            "================================"
        );

        // -----------------------------------------
        // Reset toàn bộ node
        // -----------------------------------------

        foreach (MapNode node in nodes)
        {
            if (node != null)
            {
                node.ResetNode();
            }
        }

        // -----------------------------------------
        // Tìm Start
        // -----------------------------------------

        currentNode = FindStartNode();

        if (currentNode == null)
        {
            Debug.LogError(
                "[MapManager] Không tìm thấy Start Node!"
            );

            return;
        }

        // -----------------------------------------
        // Mở Start
        // -----------------------------------------

        currentNode.SetLock(false);

        Debug.Log(
            "[MapManager] Start unlocked"
        );

        // -----------------------------------------
        // Khóa toàn bộ line
        // -----------------------------------------

        LockAllLines();
    }

    // =========================================================
    // FIND START
    // =========================================================

    private MapNode FindStartNode()
    {
        foreach (MapNode node in nodes)
        {
            if (node != null &&
                node.nodeType == NodeType.Start)
            {
                return node;
            }
        }

        return null;
    }

    // =========================================================
    // CLICK NODE
    // =========================================================

    public void SelectNode(MapNode selectedNode)
    {
        if (selectedNode == null)
        {
            Debug.LogError(
                "[MapManager] Selected Node = NULL"
            );

            return;
        }

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "[MapManager] CLICK NODE: " +
            selectedNode.gameObject.name
        );

        Debug.Log(
            "[MapManager] Type: " +
            selectedNode.nodeType
        );

        Debug.Log(
            "[MapManager] Locked: " +
            selectedNode.isLocked
        );

        Debug.Log(
            "[MapManager] Scene: " +
            selectedNode.sceneName
        );

        Debug.Log(
            "================================"
        );

        // -----------------------------------------
        // Check lock
        // -----------------------------------------

        if (selectedNode.isLocked)
        {
            Debug.Log(
                "[MapManager] Node đang khóa!"
            );

            return;
        }

        // -----------------------------------------
        // START
        // -----------------------------------------

        if (selectedNode.nodeType == NodeType.Start)
        {
            Debug.Log(
                "[MapManager] Đây là Start Node."
            );

            UnlockNextNodes(selectedNode);
            UnlockLines(selectedNode);

            currentNode = selectedNode;

            return;
        }

        // -----------------------------------------
        // BATTLE
        // -----------------------------------------

        if (selectedNode.nodeType == NodeType.Battle)
        {
            EnterBattle(selectedNode);
            return;
        }

        if (selectedNode.nodeType == NodeType.MiniBoss)
        {
            RunSession.RunActive = true;
            RunSession.IsBossBattle = true;
            RunSession.BossSequence = new List<EnemyData> { GetMiniBoss() };
            RunSession.MapSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("BattleLevel1");
            return;
        }

        if (selectedNode.nodeType == NodeType.Boss)
        {
            RunSession.RunActive = true;
            RunSession.IsBossBattle = true;
            RunSession.BossSequence = new List<EnemyData> { GetBoss() };
            RunSession.MapSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("BattleLevel1");
            return;
        }

        // -----------------------------------------
        // Các node khác
        // -----------------------------------------

        selectedNode.CompleteNode();

        currentNode = selectedNode;

        UnlockNextNodes(selectedNode);
        UnlockLines(selectedNode);

        if (!string.IsNullOrEmpty(
            selectedNode.sceneName))
        {
            LoadNodeScene(selectedNode);
        }
    }

    // =========================================================
    // ENTER BATTLE
    // =========================================================

    private void EnterBattle(MapNode battleNode)
    {
        if (battleNode == null)
            return;

        RunSession.RunActive = true;
        RunSession.IsBossBattle = false;
        RunSession.MapSceneName = SceneManager.GetActiveScene().name;

        if (string.IsNullOrWhiteSpace(
            battleNode.sceneName))
        {
            Debug.LogError(
                "[MapManager] Battle Node không có Scene Name!"
            );

            return;
        }

        // -----------------------------------------
        // Lưu Battle Node
        // -----------------------------------------

        PlayerPrefs.SetString(
            BattleNodeKey,
            battleNode.gameObject.name
        );

        PlayerPrefs.Save();

        Debug.Log(
            "[MapManager] BattleNode saved = " +
            battleNode.gameObject.name
        );

        // -----------------------------------------
        // Load Battle Scene
        // -----------------------------------------

        Debug.Log(
            "[MapManager] Load Battle Scene = " +
            battleNode.sceneName
        );

        SceneManager.LoadScene(
            battleNode.sceneName
        );
    }

    // =========================================================
    // FIND NODE
    // =========================================================

    private MapNode FindNodeByName(
        string nodeName)
    {
        foreach (MapNode node in nodes)
        {
            if (node != null &&
                node.gameObject.name == nodeName)
            {
                return node;
            }
        }

        return null;
    }

    // =========================================================
    // UNLOCK NEXT NODES
    // =========================================================

    private void UnlockNextNodes(
        MapNode selectedNode)
    {
        if (selectedNode == null)
            return;

        if (selectedNode.nextNodes == null)
            return;

        foreach (MapNode nextNode
            in selectedNode.nextNodes)
        {
            if (nextNode == null)
                continue;

            nextNode.SetLock(false);

            Debug.Log(
                "[MapManager] Unlock Node = " +
                nextNode.gameObject.name
            );
        }
    }

    // =========================================================
    // UNLOCK LINES
    // =========================================================

    private void UnlockLines(
        MapNode selectedNode)
    {
        MapLine[] lines =
            FindObjectsByType<MapLine>(
                FindObjectsSortMode.None
            );

        foreach (MapLine line in lines)
        {
            if (line == null)
                continue;

            if (line.fromNode == selectedNode)
            {
                line.Unlock();

                Debug.Log(
                    "[MapManager] Unlock Line = " +
                    line.gameObject.name
                );
            }
        }
    }

    // =========================================================
    // LOCK ALL LINES
    // =========================================================

    private void LockAllLines()
    {
        MapLine[] lines =
            FindObjectsByType<MapLine>(
                FindObjectsSortMode.None
            );

        foreach (MapLine line in lines)
        {
            if (line != null)
            {
                line.Lock();
            }
        }
    }

    // =========================================================
    // LOAD NORMAL NODE SCENE
    // =========================================================

    private void LoadNodeScene(
        MapNode node)
    {
        if (node == null)
            return;

        if (string.IsNullOrWhiteSpace(
            node.sceneName))
        {
            Debug.LogWarning(
                "[MapManager] Node không có Scene!"
            );

            return;
        }

        Debug.Log(
            "[MapManager] Load Scene = " +
            node.sceneName
        );

        SceneManager.LoadScene(
            node.sceneName
        );
    }

    // =========================================================
    // PLAYER DIED
    // =========================================================

    public void PlayerDied()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "[MapManager] PLAYER DIED"
        );

        Debug.Log(
            "[MapManager] RESET RUN"
        );

        Debug.Log(
            "================================"
        );

        // -----------------------------------------
        // Xóa Battle progress
        // -----------------------------------------

        PlayerPrefs.DeleteKey(
            BattleNodeKey
        );

        PlayerPrefs.DeleteKey(
            CompletedNodeKey
        );

        PlayerPrefs.Save();

        // -----------------------------------------
        // Reset GameProgress
        // -----------------------------------------

        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetRun();
        }

        // -----------------------------------------
        // Về Map Level 1
        // -----------------------------------------

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    public void UpdateNodes()
    {
        foreach (MapNode node in nodes)
        {
            node.SetLock(true);
        }

        if (currentNode == null)
            return;

        currentNode.SetLock(false);

        foreach (MapNode next in currentNode.nextNodes)
        {
            if (next != null)
            {
                next.SetLock(false);
            }
        }
    }

    public void CompleteNode()
    {
        if (currentNode == null)
            return;

        currentNode.isCompleted = true;

        UpdateNodes();
    }

    private void OpenRestPopup()
    {
        if (popupOpen) return;
        popupOpen = true;

        Canvas canvas = RuntimeUi.CreateCanvas("RestPopup");
        popupRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        RuntimeUi.CreateText(popupRoot.transform, "Khu nghỉ ngơi", 28, TextAnchor.MiddleCenter,
            new Vector2(0, 0.85f), new Vector2(1, 1));
        RuntimeUi.CreateButton(popupRoot.transform, "Nghỉ ngơi (+30 HP)", new Vector2(0, 100), new Vector2(340, 60), RestHeal);
        RuntimeUi.CreateButton(popupRoot.transform, "Nâng cấp 1 lá bài", new Vector2(0, 20), new Vector2(340, 60), RestUpgrade);
        RuntimeUi.CreateButton(popupRoot.transform, "Rời đi", new Vector2(0, -60), new Vector2(340, 60), ClosePopup);
    }

    private void RestHeal()
    {
        RunSession.PlayerCurrentHealth = Mathf.Min(RunSession.PlayerMaxHealth, RunSession.PlayerCurrentHealth + 30);
        CompleteNode();
        ClosePopup();
        Debug.Log("Rest: hồi 30 HP");
    }

    private void RestUpgrade()
    {
        foreach (Transform child in popupRoot.transform)
            Destroy(child.gameObject);

        if (RunSession.Deck == null || RunSession.Deck.Count == 0)
        {
            RuntimeUi.CreateText(popupRoot.transform, "Bộ bài trống!", 22, TextAnchor.MiddleCenter,
                new Vector2(0, 0.55f), new Vector2(1, 0.7f));
            RuntimeUi.CreateButton(popupRoot.transform, "Đóng", new Vector2(0, -180), new Vector2(200, 55), ClosePopup);
            return;
        }

        RuntimeUi.CreateText(popupRoot.transform, "Chọn lá bài để nâng cấp", 24, TextAnchor.MiddleCenter,
            new Vector2(0, 0.8f), new Vector2(1, 0.95f));

        int shown = Mathf.Min(RunSession.Deck.Count, 12);
        for (int i = 0; i < shown; i++)
        {
            CardData card = RunSession.Deck[i];
            string label = card.cardName + (card.isUpgraded ? " (Đã nâng cấp)" : "");
            RuntimeUi.CreateButton(popupRoot.transform, label,
                new Vector2(0, 240 - i * 48),
                new Vector2(400, 44),
                () => UpgradeChosen(card));
        }
    }

    private void UpgradeChosen(CardData card)
    {
        card.Upgrade();
        CompleteNode();
        ClosePopup();
        Debug.Log("Đã nâng cấp: " + card.cardName);
    }

    private void OpenChestPopup()
    {
        if (popupOpen) return;
        popupOpen = true;

        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        CardData reward = db != null ? db.GetRandomCard() : RuntimeCardLibrary.GetRandomCard();

        Canvas canvas = RuntimeUi.CreateCanvas("ChestPopup");
        popupRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        RuntimeUi.CreateText(popupRoot.transform, "Rương báu vật!", 30, TextAnchor.MiddleCenter,
            new Vector2(0, 0.78f), new Vector2(1, 0.95f));

        if (reward != null)
        {
            RunSession.Deck.Add(reward);
            RuntimeUi.CreateText(popupRoot.transform, $"Nhận được: {reward.cardName}\n{reward.description}", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.45f), new Vector2(1, 0.7f));
        }
        else
        {
            RuntimeUi.CreateText(popupRoot.transform, "Rương trống rỗng...", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.5f), new Vector2(1, 0.65f));
        }

        RuntimeUi.CreateButton(popupRoot.transform, "Nhận", new Vector2(0, -220), new Vector2(220, 60), () =>
        {
            CompleteNode();
            ClosePopup();
        });
    }

    private void ClosePopup()
    {
        popupOpen = false;
        if (popupRoot != null)
        {
            GameObject toDestroy = popupRoot.transform.parent != null ? popupRoot.transform.parent.gameObject : popupRoot;
            Destroy(toDestroy);
            popupRoot = null;
        }
    }

    private EnemyData GetMiniBoss()
    {
        if (miniBossEnemy != null) return miniBossEnemy;
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.enemyName = "Mini Boss Knight";
        d.archetype = EnemyArchetype.Knight;
        d.maxHealth = 90;
        d.attackDamage = 12;
        d.block = 10;
        d.selfHeal = 10;
        d.isBoss = true;
        return d;
    }

    private EnemyData GetBoss()
    {
        if (bossEnemy != null) return bossEnemy;
        EnemyData d = ScriptableObject.CreateInstance<EnemyData>();
        d.enemyName = "Boss Golem";
        d.archetype = EnemyArchetype.Golem;
        d.maxHealth = 150;
        d.attackDamage = 14;
        d.block = 8;
        d.selfHeal = 20;
        d.regenValue = 6;
        d.isBoss = true;
        return d;
    }
}