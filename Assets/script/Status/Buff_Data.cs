using UnityEngine;

public class Buff_Data : MonoBehaviour
{
    
    public enum buffType
    {
        Buff,
        Debuff
    }
    [CreateAssetMenu(fileName = "Buff_Data", menuName = "Buff_Data")]
    public class BuffData : ScriptableObject
    {
        [Header("info")]
        public string BuffID;
        public string BuffName;
        public Sprite BuffIcon;
        public buffType Type;
        [Header ("description")]
        [TextArea] public string Description;
    }

}
