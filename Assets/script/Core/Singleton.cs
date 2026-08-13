using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            bool existingAlive = Instance != null;
            int existingId = existingAlive ? (Instance as MonoBehaviour).GetInstanceID() : 0;
            Debug.LogWarning(
                $"[Singleton<{typeof(T).Name}>] Duplicate Awake on '{gameObject.name}' (id={GetInstanceID()}). " +
                $"Existing alive={existingAlive}, id={existingId}. Destroying this duplicate."
            );
            Destroy(gameObject);
        }
    }
}
