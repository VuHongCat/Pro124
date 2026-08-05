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

    public const string BattleNodeKey = "BattleNode";
    private const string CompletedNodeKey = "CompletedMapNode";
    private const char NodeSeparator = ';';

    // =========================================================
    // PROGRESS PERSISTENCE
    // =========================================================

    public static void SaveCompletedNode(string nodeName)
    {
        if (string.IsNullOrEmpty(nodeName)) return;

        List<string> names = GetCompletedNodeNames();
        if (!names.Contains(nodeName))
            names.Add(nodeName);

        PlayerPrefs.SetString(
            CompletedNodeKey,
            string.Join(NodeSeparator.ToString(), names)
        );

        PlayerPrefs.Save();
    }

    private static List<string> GetCompletedNodeNames()
    {
        List<string> result = new();

        string raw = PlayerPrefs.GetString(
            CompletedNodeKey,
            ""
        );

        if (string.IsNullOrEmpty(raw))
            return result;

        foreach (string name in raw.Split(NodeSeparator))
        {
            string trimmed = name.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                result.Add(trimmed);
        }

        return result;
    }

    public static void ClearProgress()
    {
        PlayerPrefs.DeleteKey(CompletedNodeKey);
        PlayerPrefs.DeleteKey(BattleNodeKey);
        PlayerPrefs.Save();
    }

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
        // Chưa có run đang chạy -> xóa progress cũ
        // để map luôn bắt đầu từ Start
        if (!RunSession.RunActive)
        {
            ClearProgress();
        }

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
        List<string> completedNames = GetCompletedNodeNames();

        // -----------------------------------------
        // Reset toàn bộ node (chỉ Start được mở)
        // -----------------------------------------

        foreach (MapNode node in nodes)
        {
            if (node != null)
            {
                node.ResetNode();
            }
        }

        // -----------------------------------------
        // Khóa toàn bộ line trước
        // -----------------------------------------

        LockAllLines();

        // -----------------------------------------
        // Thắng trận quay lại map: BattleNodeKey vẫn giữ
        // tên node vừa đánh -> đánh dấu node đó hoàn thành
        // -----------------------------------------

        string pendingBattle = PlayerPrefs.GetString(BattleNodeKey, "");

        if (!string.IsNullOrEmpty(pendingBattle) &&
            !completedNames.Contains(pendingBattle))
        {
            MapNode battleNode = FindNodeByName(pendingBattle);

            if (battleNode != null)
            {
                battleNode.isCompleted = true;
                SaveCompletedNode(pendingBattle);
                completedNames = GetCompletedNodeNames();
            }
        }

        // -----------------------------------------
        // Chưa có progress: chỉ Start sáng
        // -----------------------------------------

        if (completedNames.Count == 0)
        {
            currentNode = FindStartNode();

            if (currentNode != null)
            {
                currentNode.SetLock(false);
            }

            return;
        }

        // -----------------------------------------
        // Node cuối cùng đã hoàn thành = vị trí hiện tại
        // -----------------------------------------

        MapNode lastCompleted = FindNodeByName(
            completedNames[completedNames.Count - 1]
        );

        if (lastCompleted == null)
        {
            Debug.LogWarning(
                "[MapManager] Completed Node not found: " +
                completedNames[completedNames.Count - 1]
            );

            ResetMapToStart();
            return;
        }

        currentNode = lastCompleted;

        Debug.Log(
            "[MapManager] Restore progress, currentNode = " +
            currentNode.gameObject.name
        );

        // -----------------------------------------
        // Start chỉ sáng khi nó là node hiện tại
        // (ResetNode luôn mở Start, nên khóa lại
        // nếu người chơi đang ở node khác)
        // -----------------------------------------

        MapNode startNode = FindStartNode();

        if (startNode != null && startNode != currentNode)
        {
            startNode.SetLock(true);
        }

        // -----------------------------------------
        // Sáng node hiện tại (vị trí người chơi)
        // -----------------------------------------

        currentNode.SetLock(false);

        // -----------------------------------------
        // Tự mở khóa nhánh kế tiếp để người chơi
        // chọn ngay sau khi hoàn thành node hiện tại
        // -----------------------------------------

        UnlockNextNodes(currentNode);
        UnlockLines(currentNode);
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
                "[MapManager] Start Node not found!"
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
                "[MapManager] Node is locked!"
            );

            return;
        }

        // -----------------------------------------
        // Bấm vào node hiện tại (vị trí người chơi)
        // -> mở khóa các nhánh kế tiếp
        // -----------------------------------------

        if (selectedNode == currentNode)
        {
            Debug.Log(
                "[MapManager] Unlocked next nodes for: " +
                selectedNode.gameObject.name
            );

            UnlockNextNodes(selectedNode);
            UnlockLines(selectedNode);

            return;
        }

        // -----------------------------------------
        // START
        // -----------------------------------------

        if (selectedNode.nodeType == NodeType.Start)
        {
            Debug.Log(
                "[MapManager] This is the Start Node."
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
            PlayerPrefs.SetString(BattleNodeKey, selectedNode.gameObject.name);
            PlayerPrefs.Save();

            RunSession.RunActive = true;
            RunSession.IsBossBattle = true;
            RunSession.BossSequence = new List<EnemyData> { GetMiniBoss() };
            RunSession.MapSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("BattleLevel1");
            return;
        }

        if (selectedNode.nodeType == NodeType.Boss)
        {
            PlayerPrefs.SetString(BattleNodeKey, selectedNode.gameObject.name);
            PlayerPrefs.Save();

            RunSession.RunActive = true;
            RunSession.IsBossBattle = true;
            RunSession.BossSequence = new List<EnemyData> { GetBoss() };
            RunSession.MapSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("BattleLevel1");
            return;
        }

        // -----------------------------------------
        // REST
        // -----------------------------------------

        if (selectedNode.nodeType == NodeType.Rest)
        {
            currentNode = selectedNode;
            OpenRestPopup();
            return;
        }

        // -----------------------------------------
        // CHEST
        // -----------------------------------------

        if (selectedNode.nodeType == NodeType.Chest)
        {
            currentNode = selectedNode;
            OpenChestPopup();
            return;
        }

        // -----------------------------------------
        // SHOP
        // -----------------------------------------

        if (selectedNode.nodeType == NodeType.Shop)
        {
            PlayerPrefs.SetString(BattleNodeKey, selectedNode.gameObject.name);
            PlayerPrefs.Save();

            RunSession.RunActive = true;
            RunSession.MapSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("Shop");
            return;
        }

        // -----------------------------------------
        // Các node khác
        // -----------------------------------------

        currentNode = selectedNode;

        selectedNode.CompleteNode();
        SaveCompletedNode(selectedNode.gameObject.name);

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
                "[MapManager] Battle Node has no Scene Name!"
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
                "[MapManager] Node has no Scene!"
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

        ClearProgress();

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
        UpdateMapFromProgress();
    }

    public void CompleteNode()
    {
        if (currentNode == null)
            return;

        currentNode.isCompleted = true;

        SaveCompletedNode(currentNode.gameObject.name);

        UpdateNodes();
    }

    private void OpenRestPopup()
    {
        if (popupOpen) return;
        popupOpen = true;

        Canvas canvas = RuntimeUi.CreateCanvas("RestPopup");
        popupRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        RuntimeUi.CreateText(popupRoot.transform, "Rest Area", 28, TextAnchor.MiddleCenter,
            new Vector2(0, 0.85f), new Vector2(1, 1));

        if (RelicManager.Owns("Coffee Dripper"))
        {
            RuntimeUi.CreateText(popupRoot.transform, "Coffee Dripper: cannot rest here!",
                18, TextAnchor.MiddleCenter, new Vector2(0, 0.52f), new Vector2(1, 0.62f));
        }
        else
        {
            RuntimeUi.CreateButton(popupRoot.transform, "Rest (+30 HP)", new Vector2(0, 100), new Vector2(340, 60), RestHeal);
        }

        RuntimeUi.CreateButton(popupRoot.transform, "Upgrade 1 card", new Vector2(0, 20), new Vector2(340, 60), RestUpgrade);
        RuntimeUi.CreateButton(popupRoot.transform, "Leave", new Vector2(0, -60), new Vector2(340, 60), ClosePopup);
    }

    private void RestHeal()
    {
        RelicManager.EmitRestSite();

        RunSession.PlayerCurrentHealth = Mathf.Min(RunSession.PlayerMaxHealth, RunSession.PlayerCurrentHealth + 30);
        CompleteNode();
        ClosePopup();
        Debug.Log("Rest: +30 HP");
    }

    private void RestUpgrade()
    {
        foreach (Transform child in popupRoot.transform)
            Destroy(child.gameObject);

        if (RunSession.Deck == null || RunSession.Deck.Count == 0)
        {
            RuntimeUi.CreateText(popupRoot.transform, "Deck is empty!", 22, TextAnchor.MiddleCenter,
                new Vector2(0, 0.55f), new Vector2(1, 0.7f));
            RuntimeUi.CreateButton(popupRoot.transform, "Close", new Vector2(0, -180), new Vector2(200, 55), ClosePopup);
            return;
        }

        RuntimeUi.CreateText(popupRoot.transform, "Choose a card to upgrade", 24, TextAnchor.MiddleCenter,
            new Vector2(0, 0.8f), new Vector2(1, 0.95f));

        int shown = Mathf.Min(RunSession.Deck.Count, 12);
        for (int i = 0; i < shown; i++)
        {
            CardData card = RunSession.Deck[i];
            string label = card.cardName + (card.isUpgraded ? " (Upgraded)" : "");
            RuntimeUi.CreateButton(popupRoot.transform, label,
                new Vector2(0, 240 - i * 48),
                new Vector2(400, 44),
                () => UpgradeChosen(card));
        }
    }

    private void UpgradeChosen(CardData card)
    {
        RelicManager.EmitRestSite();

        card.Upgrade();
        CompleteNode();
        ClosePopup();
        Debug.Log("Upgraded: " + card.cardName);
    }

    private void OpenChestPopup()
    {
        if (popupOpen) return;
        popupOpen = true;

        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        CardData reward = db != null ? db.GetRandomCard() : RuntimeCardLibrary.GetRandomCard();

        Canvas canvas = RuntimeUi.CreateCanvas("ChestPopup");
        popupRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        RuntimeUi.CreateText(popupRoot.transform, "Treasure Chest!", 30, TextAnchor.MiddleCenter,
            new Vector2(0, 0.78f), new Vector2(1, 0.95f));

        if (reward != null)
        {
            RunSession.Deck.Add(reward);
            RelicManager.EmitObtainCard(reward);
            RuntimeUi.CreateText(popupRoot.transform, $"Received: {reward.cardName}\n{reward.description}", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.45f), new Vector2(1, 0.7f));
        }
        else
        {
            RuntimeUi.CreateText(popupRoot.transform, "The chest is empty...", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.5f), new Vector2(1, 0.65f));
        }

        RuntimeUi.CreateButton(popupRoot.transform, "Take", new Vector2(0, -220), new Vector2(220, 60), () =>
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