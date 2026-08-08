using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class RuntimeUi
{
    public static void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        GameObject go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    public static Canvas CreateCanvas(string name)
    {
        EnsureEventSystem();
        GameObject go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        return canvas;
    }

    public static GameObject CreatePanel(Transform parent, Color color)
    {
        GameObject go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        go.GetComponent<Image>().color = color;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    public static Text CreateText(Transform parent, string text, int size, TextAnchor align, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        Text t = go.GetComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.alignment = align;
        t.color = Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return t;
    }

    public static Button CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, Action onClick, bool interactable = true)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.one * 0.5f;
        rt.anchorMax = Vector2.one * 0.5f;
        rt.pivot = Vector2.one * 0.5f;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.interactable = interactable;
        if (!interactable)
        {
            ColorBlock cb = btn.colors;
            cb.disabledColor = new Color(0.45f, 0.45f, 0.5f, 0.7f);
            btn.colors = cb;
        }
        if (interactable)
            btn.onClick.AddListener(() => onClick());
        CreateText(go.transform, label, 18, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        return btn;
    }
}
