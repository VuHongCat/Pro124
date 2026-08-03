using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Current Map Level")]
    public int levelNumber = 1;

    [Header("All Nodes")]
    public List<MapNode> nodes = new List<MapNode>();

    [Header("Current Node")]
    public MapNode currentNode;

    [Header("Boss Enemies")]
    public EnemyData miniBossEnemy;
    public EnemyData bossEnemy;

    private GameObject popupRoot;
    private bool popupOpen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindNodes();
        FindStartNode();
        UpdateNodes();
    }

    void FindNodes()
    {
        nodes.Clear();

        MapNode[] allNodes =
            FindObjectsByType<MapNode>(FindObjectsSortMode.None);

        nodes.AddRange(allNodes);

        Debug.Log("Found " + nodes.Count + " nodes");
    }

    void FindStartNode()
    {
        if (currentNode != null)
            return;

        foreach (MapNode node in nodes)
        {
            if (node.nodeType == NodeType.Start)
            {
                currentNode = node;
                Debug.Log("Start Node: " + node.name);
                break;
            }
        }
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

    public void SelectNode(MapNode node)
    {
        if (node.isLocked)
            return;

        currentNode = node;

        UpdateNodes();

        switch (node.nodeType)
        {
            case NodeType.Start:
                Debug.Log("Start");
                break;

            case NodeType.Battle:
                RunSession.RunActive = true;
                RunSession.IsBossBattle = false;
                RunSession.MapSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("Battle");
                break;

            case NodeType.Shop:
                Debug.Log("Open Shop UI");
                // ShopUI.SetActive(true);
                break;

            case NodeType.Chest:
                OpenChestPopup();
                break;

            case NodeType.Rest:
                OpenRestPopup();
                break;

            case NodeType.MiniBoss:
                RunSession.RunActive = true;
                RunSession.IsBossBattle = true;
                RunSession.BossSequence = new List<EnemyData> { GetMiniBoss() };
                RunSession.MapSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("Battle");
                break;

            case NodeType.Boss:
                RunSession.RunActive = true;
                RunSession.IsBossBattle = true;
                RunSession.BossSequence = new List<EnemyData> { GetBoss() };
                RunSession.MapSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene("Battle");
                break;

            default:
                Debug.LogWarning("Unknown Node Type");
                break;
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