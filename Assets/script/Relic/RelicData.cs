using UnityEngine;

[CreateAssetMenu(fileName = "RelicData", menuName = "Scriptable Objects/Relic Data")]
public class RelicData : ScriptableObject
{
    [Header("Basic")]
    public string relicName;

    [TextArea]
    public string description;

    public Sprite icon;

    public RelicRarity rarity;

    public RelicType relicType;

    [Header("Value")]
    public int value;

    [Header("Special")]
    public int secondValue;

    public bool stackable = false;

    [Header("Shop")]
    public int shopPrice = 150;
}
