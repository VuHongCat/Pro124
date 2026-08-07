using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [SerializeField] private float magnitude = 18f;
    [SerializeField] private float duration = 0.12f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 origin = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float m = magnitude * (1f - t);
            transform.localPosition = origin + new Vector3(
                Random.Range(-m, m),
                Random.Range(-m, m),
                0f);
            yield return null;
        }
        transform.localPosition = origin;
    }
}
