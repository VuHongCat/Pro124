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

    private Vector3[] basePositions;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        Instance = this;
        Debug.Log($"[ScreenShake] Awake on {gameObject.name}, Instance set");
        CaptureBase();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void CaptureBase()
    {
        if (targets == null || targets.Length == 0)
            targets = new[] { transform };

        basePositions = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; i++)
            if (targets[i] != null)
                basePositions[i] = targets[i].localPosition;
    }

    public void Shake()
    {
        Debug.Log($"[ScreenShake] Shake() called on {gameObject.name}, Instance null? {Instance == null}");
        if (gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning("[ScreenShake] GameObject inactive, shake skipped");
            return;
        }

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        if (basePositions == null)
            CaptureBase();

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
                    targets[i].localPosition = basePositions[i] + offset;
            }

            yield return null;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
                targets[i].localPosition = basePositions[i];
        }

        shakeRoutine = null;
    }
}
