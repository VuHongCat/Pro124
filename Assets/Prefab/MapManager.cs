using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("All Map Nodes")]
    public List<MapNode> nodes = new List<MapNode>();

    [Header("Current Node")]
    public MapNode currentNode;

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
}