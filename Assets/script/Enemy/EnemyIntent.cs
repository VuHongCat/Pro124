using UnityEngine;
using System;

public class EnemyIntent : MonoBehaviour
{
    public EnemyIntentType IntentType {  get; private set; }
    public int IntentValue {  get; private set; }
    public event Action<EnemyIntentType, int> OnIntentChanged;

    public void SetIntent(EnemyIntentType type, int value)
    {
        IntentType = type;
        IntentValue = value;

        OnIntentChanged?.Invoke(type, value);
    }
}
