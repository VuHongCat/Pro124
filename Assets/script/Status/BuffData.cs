using UnityEngine;

public enum BuffType
{
    Buff,
    Debuff
}

[CreateAssetMenu(fileName = "BuffData", menuName = "BuffData")]
public class BuffData : ScriptableObject
{
    [Header("info")]
    public string BuffID;
    public string BuffName;
    public Sprite BuffIcon;
    public BuffType Type;
    [Header("description")]
    [TextArea] public string Description;
}
