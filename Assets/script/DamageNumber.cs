using System.Collections;
using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private float floatDistance = 45f;
    [SerializeField] private float duration = 0.75f;
    [SerializeField] private float startDelay = 0.05f;
    [SerializeField] private float popDuration = 0.3f;

    private TextMeshProUGUI label;

    private void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Play(int amount)
    {
        if (label != null)
            label.text = amount.ToString();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(startDelay);

        Vector3 origin = transform.localPosition;
        Vector3 end = origin + new Vector3(0f, floatDistance, 0f);
        Color c = label != null ? label.color : Color.white;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float pop = Mathf.Clamp01(t / popDuration);
            float scale = 0.3f + 0.7f * EaseOutBack(pop);
            transform.localScale = new Vector3(scale, scale, 1f);

            float rise = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t * 1.4f));
            transform.localPosition = Vector3.Lerp(origin, end, rise);

            transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * 45f) * 6f * (1f - t));

            if (label != null)
            {
                float fadeStart = 0.6f;
                float a = t < fadeStart ? 1f : 1f - (t - fadeStart) / (1f - fadeStart);
                label.color = new Color(c.r, c.g, c.b, c.a * a);
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
