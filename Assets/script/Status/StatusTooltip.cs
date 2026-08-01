using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class StatusTooltip
{
    private static GameObject root;
    private static RectTransform rect;
    private static TMP_Text titleText;
    private static TMP_Text descText;
    private static Canvas canvas;
    private static RectTransform canvasRect;
    private static RectTransform anchorRect;
    private static bool showing;

    private static void EnsureCreated(Canvas ownerCanvas)
    {
        if (root != null) return;

        canvas = ownerCanvas;
        if (canvas == null)
            canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new("StatusTooltipCanvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
        }
        canvasRect = (RectTransform)canvas.transform;

        root = new GameObject("StatusTooltip", typeof(RectTransform));
        rect = (RectTransform)root.transform;
        rect.SetParent(canvas.transform, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(220f, 90f);

        Image background = root.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.88f);
        background.raycastTarget = false;

        GameObject titleGo = new("Title", typeof(RectTransform));
        titleText = titleGo.AddComponent<TextMeshProUGUI>();
        RectTransform titleRect = (RectTransform)titleGo.transform;
        titleRect.SetParent(rect, false);
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(8f, -30f);
        titleRect.offsetMax = new Vector2(-8f, -4f);
        titleText.fontSize = 18;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.MidlineLeft;
        titleText.raycastTarget = false;

        GameObject descGo = new("Description", typeof(RectTransform));
        descText = descGo.AddComponent<TextMeshProUGUI>();
        RectTransform descRect = (RectTransform)descGo.transform;
        descRect.SetParent(rect, false);
        descRect.anchorMin = new Vector2(0f, 0f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.offsetMin = new Vector2(8f, 6f);
        descRect.offsetMax = new Vector2(-8f, -34f);
        descText.fontSize = 14;
        descText.color = new Color(0.85f, 0.85f, 0.85f, 1f);
        descText.alignment = TextAlignmentOptions.TopLeft;
        descText.textWrappingMode = TextWrappingModes.Normal;
        descText.raycastTarget = false;

        root.SetActive(false);
    }

    public static void Show(string title, string description, RectTransform anchor)
    {
        Canvas owner = anchor != null ? anchor.GetComponentInParent<Canvas>() : null;
        EnsureCreated(owner);
        titleText.text = title;
        descText.text = string.IsNullOrEmpty(description) ? "No description" : description;
        anchorRect = anchor;
        root.SetActive(true);
        showing = true;
        PositionNextToAnchor();
    }

    public static void Hide()
    {
        if (root != null && root.activeSelf)
            root.SetActive(false);
        showing = false;
        anchorRect = null;
    }

    public static void Tick()
    {
        if (showing && root != null && root.activeSelf)
            PositionNextToAnchor();
    }

    private static void PositionNextToAnchor()
    {
        if (canvasRect == null || anchorRect == null) return;

        Vector3[] corners = new Vector3[4];
        anchorRect.GetWorldCorners(corners);

        Vector2 leftBottom = WorldToScreen(corners[0]);
        Vector2 rightTop = WorldToScreen(corners[2]);
        Vector2 center = WorldToScreen(corners[0] + (corners[2] - corners[0]) * 0.5f);

        Vector2 size = rect.sizeDelta;
        float screenW = canvasRect.rect.width;
        float screenH = canvasRect.rect.height;
        float gap = 8f;

        float x = rightTop.x + gap;
        if (x + size.x > screenW)
            x = leftBottom.x - gap - size.x;
        x = Mathf.Clamp(x, 0f, Mathf.Max(0f, screenW - size.x));

        float y = center.y + size.y * 0.5f;
        y = Mathf.Clamp(y, Mathf.Min(size.y, screenH), screenH);

        rect.anchoredPosition = new Vector2(x, y);
    }

    private static Vector2 WorldToScreen(Vector3 world)
    {
        if (canvas == null) return Vector2.zero;
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return RectTransformUtility.WorldToScreenPoint(null, world);
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        return RectTransformUtility.WorldToScreenPoint(cam, world);
    }
}
