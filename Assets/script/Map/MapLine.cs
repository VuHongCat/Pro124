using UnityEngine;

public class MapLine : MonoBehaviour
{
    public MapNode fromNode;
    public MapNode toNode;

    private SpriteRenderer sr;

    public Color lockColor = Color.gray;
    public Color unlockColor = Color.white;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Lock()
    {
        sr.color = lockColor;
    }

    public void Unlock()
    {
        sr.color = unlockColor;
    }
}