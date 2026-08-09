using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [SerializeField] private float magnitude = 1f;
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private float decay = 3.5f;
    [SerializeField] private float frequency = 22f;
    [SerializeField] private Transform[] targets;

    private void Awake()
    {
        Instance = this;
        Debug.Log($"[ScreenShake] Awake on {gameObject.name}, Instance set");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Shake()
    {
        Debug.Log($"[ScreenShake] Shake() called on {gameObject.name}, Instance null? {Instance == null}");
        if (gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning("[ScreenShake] GameObject inactive, shake skipped");
            return;
        }
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        if (targets == null || targets.Length == 0)
            targets = new[] { transform };

        Vector3[] origins = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; i++)
            if (targets[i] != null)
                origins[i] = targets[i].localPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float amp = Mathf.Exp(-decay * t) * magnitude;
            float x = Mathf.Sin(elapsed * frequency) * amp;
            float y = Mathf.Cos(elapsed * frequency * 0.85f) * amp;
            Vector3 offset = new Vector3(x, y, 0f);

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null)
                    targets[i].localPosition = origins[i] + offset;
            }

            yield return null;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
                targets[i].localPosition = origins[i];
        }
    }
}
