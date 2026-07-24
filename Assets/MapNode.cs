using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    [Header("Node Type")]
    public NodeType nodeType;

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
    }

    public void SetLock(bool value)
    {
        isLocked = value;
        UpdateColor();
    }

    void UpdateColor()
    {
        if (sr == null) return;

        if (isLocked)
            sr.color = Color.gray;
        else
            sr.color = Color.white;
    }

    private void OnMouseDown()
    {
        Debug.Log("Click: " + gameObject.name);

        if (isLocked)
        {
            Debug.Log("Node đang khóa");
            return;
        }

        if (MapManager.instance == null)
        {
            Debug.LogError("Không tìm thấy MapManager");
            return;
        }

        MapManager.instance.SelectNode(this);
    }
}