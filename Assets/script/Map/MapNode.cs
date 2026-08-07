using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    [Header("Node Type")]
    public NodeType nodeType;

    [Header("Scene Name")]
    public string sceneName;

    [Header("Next Nodes")]
    public List<MapNode> nextNodes = new List<MapNode>();

    [Header("Status")]
    public bool isLocked = true;
    public bool isCompleted = false;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateColor();

        Debug.Log(
            "[MapNode] " +
            gameObject.name +
            " | Type = " +
            nodeType +
            " | Scene = " +
            sceneName +
            " | Locked = " +
            isLocked
        );
    }

    // ==========================================
    // LOCK / UNLOCK
    // ==========================================

    public void SetLock(bool value)
    {
        isLocked = value;

        UpdateColor();

        Debug.Log(
            "[MapNode] " +
            gameObject.name +
            " -> Locked = " +
            isLocked
        );
    }

    // ==========================================
    // COMPLETE
    // ==========================================

    public void CompleteNode()
    {
        isCompleted = true;

        Debug.Log(
            "[MapNode] Completed: " +
            gameObject.name
        );
    }

    // ==========================================
    // RESET
    // ==========================================

    public void ResetNode()
    {
        isCompleted = false;

        if (nodeType == NodeType.Start)
        {
            isLocked = false;
        }
        else
        {
            isLocked = true;
        }

        UpdateColor();
    }

    // ==========================================
    // COLOR
    // ==========================================

    private void UpdateColor()
    {
        if (sr == null)
            return;

        if (isLocked)
        {
            sr.color = Color.gray;
        }
        else
        {
            sr.color = Color.white;
        }
    }

    // ==========================================
    // CLICK
    // ==========================================

    private void OnMouseDown()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "[MapNode] CLICKED: " +
            gameObject.name
        );

        Debug.Log(
            "[MapNode] Type: " +
            nodeType
        );

        Debug.Log(
            "[MapNode] Locked: " +
            isLocked
        );

        Debug.Log(
            "[MapNode] Scene: " +
            sceneName
        );

        Debug.Log(
            "================================"
        );

        // Node đang khóa
        if (isLocked)
        {
            Debug.LogWarning(
                "[MapNode] NODE IS LOCKED!"
            );

            return;
        }

        // Kiểm tra MapManager
        if (MapManager.instance == null)
        {
            Debug.LogError(
                "[MapNode] MAP MANAGER NOT FOUND!"
            );

            return;
        }

        Debug.Log(
            "[MapNode] Calling MapManager.SelectNode()"
        );

        MapManager.instance.SelectNode(this);
    }
}