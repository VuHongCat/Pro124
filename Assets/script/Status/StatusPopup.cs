using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusPopup : MonoBehaviour
{
    [SerializeField] private float floatDistance = 60f;
    [SerializeField] private float duration = 1.1f;
    [SerializeField] private float startDelay = 0.05f;
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private Sprite buffSprite;
    [SerializeField] private Sprite debuffSprite;

    private Image iconImage;
    private TMP_Text stackText;
    private BuffType popupType;

    private void Awake()
    {
        iconImage = GetComponentInChildren<Image>();
        stackText = GetComponentInChildren<TMP_Text>();
    }

    public void Play(BuffType type, int stacks)
    {
        Sprite icon = type == BuffType.Buff ? buffSprite : debuffSprite;
        if (iconImage != null && icon != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        if (stackText != null)
            stackText.text = stacks.ToString();
        popupType = type;
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(startDelay);

        Vector3 origin = transform.localPosition;
        float dir = popupType == BuffType.Debuff ? -1f : 1f;
        Vector3 end = origin + new Vector3(0f, floatDistance * dir, 0f);
        Color iconColor = iconImage != null ? iconImage.color : Color.white;
        Color textColor = stackText != null ? stackText.color : Color.white;

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

            float fadeStart = 0.55f;
            float a = t < fadeStart ? 1f : 1f - (t - fadeStart) / (1f - fadeStart);
            if (iconImage != null)
                iconImage.color = new Color(iconColor.r, iconColor.g, iconColor.b, iconColor.a * a);
            if (stackText != null)
                stackText.color = new Color(textColor.r, textColor.g, textColor.b, textColor.a * a);

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