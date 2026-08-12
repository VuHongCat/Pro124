using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [Header("Colors")]
    public Color lockedColor = new Color(0.32f, 0.32f, 0.35f, 1f);
    public Color unlockedColor = Color.white;
    public Color completedColor = new Color(0.18f, 0.78f, 0.28f, 1f);

    private SpriteRenderer sr;
    private GameObject tooltipGo;

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

        UpdateColor();

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

        if (isCompleted)
        {
            sr.color = completedColor;
        }
        else if (isLocked)
        {
            sr.color = lockedColor;
        }
        else
        {
            sr.color = unlockedColor;
        }
    }

    // ==========================================
    // HOVER (thông báo node bị khóa)
    // ==========================================

    private void OnMouseEnter()
    {
        if (isLocked && !IsPointerOverUi())
            ShowLockedTooltip();
    }

    private void OnMouseExit()
    {
        HideLockedTooltip();
    }

    // True khi con trỏ đang trỏ vào UI (panel Monster Index, popup...)
    // -> chặn không cho tương tác với node map bên dưới
    private static bool IsPointerOverUi()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    private void ShowLockedTooltip()
    {
        if (tooltipGo != null)
            return;

        Canvas canvas = RuntimeUi.CreateCanvas("LockedTooltip");
        Text t = RuntimeUi.CreateText(canvas.transform, "Locked", 26, TextAnchor.MiddleCenter,
            new Vector2(0.3f, 0.88f), new Vector2(0.7f, 0.96f));
        t.raycastTarget = false;

        tooltipGo = canvas.gameObject;
    }

    private void HideLockedTooltip()
    {
        if (tooltipGo == null)
            return;

        Destroy(tooltipGo);
        tooltipGo = null;
    }

    // ==========================================
    // CLICK
    // ==========================================

    private void OnMouseDown()
    {
        // Đang trỏ vào UI (panel Monster Index, popup...) -> không tương tác node map
        if (IsPointerOverUi())
            return;

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