using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockPopup : MonoBehaviour
{
    [SerializeField] private float floatDistance = 55f;
    [SerializeField] private float duration = 0.9f;
    [SerializeField] private float startDelay = 0.05f;
    [SerializeField] private float popDuration = 0.3f;

    private TextMeshProUGUI label;
    private Image icon;

    private void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
        icon = GetComponent<Image>();
    }

    public void Play(int amount)
    {
        Play(amount, false);
    }

    public void Play(int amount, bool stayCenter)
    {
        if (label != null)
            label.text = amount.ToString();
        StartCoroutine(Animate(stayCenter));
    }

    private IEnumerator Animate(bool stayCenter)
    {
        yield return new WaitForSeconds(startDelay);

        Vector3 origin = transform.localPosition;
        Vector3 end = stayCenter ? origin : origin + new Vector3(0f, floatDistance, 0f);
        Color iconColor = icon != null ? icon.color : Color.white;
        Color textColor = label != null ? label.color : Color.white;

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

            float fadeStart = 0.6f;
            float a = t < fadeStart ? 1f : 1f - (t - fadeStart) / (1f - fadeStart);
            if (icon != null)
                icon.color = new Color(iconColor.r, iconColor.g, iconColor.b, iconColor.a * a);
            if (label != null)
                label.color = new Color(textColor.r, textColor.g, textColor.b, textColor.a * a);

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
