using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnBanner : MonoBehaviour
{
    private static TurnBanner instance;

    private Text text;

    public static void Show(string message, Color color)
    {
        if (instance == null)
        {
            GameObject go = new GameObject("TurnBanner");
            go.AddComponent<Canvas>();
            Canvas canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            instance = go.AddComponent<TurnBanner>();

            GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            RectTransform rt = textGo.GetComponent<RectTransform>();
            rt.SetParent(go.transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Text t = textGo.GetComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 72;
            t.fontStyle = FontStyle.Bold;
            t.alignment = TextAnchor.MiddleCenter;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            instance.text = t;
        }

        instance.StartCoroutine(instance.Play(message, color));
    }

    private IEnumerator Play(string message, Color color)
    {
        text.text = message;
        text.color = new Color(color.r, color.g, color.b, 0f);

        float hold = 0.5f;
        float fadeIn = 0.15f;
        float fadeOut = 0.4f;

        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeIn);
            text.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        t = 0f;
        while (t < hold)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / fadeOut);
            text.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }

        text.color = new Color(color.r, color.g, color.b, 0f);
    }
}
