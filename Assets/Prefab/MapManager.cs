using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Current Map Level")]
    public int levelNumber = 1;

    [Header("All Nodes")]
    public List<MapNode> nodes = new List<MapNode>();

    [Header("Current Node")]
    public MapNode currentNode;

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
                SceneManager.LoadScene("BattleLevel" + levelNumber);
                break;

            case NodeType.Shop:
                Debug.Log("Open Shop UI");
                // ShopUI.SetActive(true);
                break;

            case NodeType.Chest:
                Debug.Log("Open Chest");
                // Reward
                break;

            case NodeType.Rest:
                Debug.Log("Rest");
                // Heal Player
                break;

            case NodeType.MiniBoss:
                SceneManager.LoadScene("MiniBoss");
                break;

            case NodeType.Boss:
                SceneManager.LoadScene("BossLevel" + levelNumber);
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
}