using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RelicBarUI : MonoBehaviour
{
    public static RelicBarUI Instance;

    private RectTransform barRoot;
    private GameObject tooltipPanel;
    private Text tooltipName;
    private Text tooltipDesc;
    private RectTransform tooltipRt;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildUI();

        RelicManager.RelicAdded += OnRelicAdded;
    }

    private void OnDestroy()
    {
        RelicManager.RelicAdded -= OnRelicAdded;

        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        Refresh();
    }

    private void BuildUI()
    {
        GameObject canvasGo = new GameObject(
            "RelicBarCanvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );
        canvasGo.transform.SetParent(transform, false);

        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        GameObject barGo = new GameObject("RelicBar", typeof(RectTransform));
        barRoot = barGo.GetComponent<RectTransform>();
        barRoot.SetParent(canvasGo.transform, false);
        barRoot.anchorMin = new Vector2(0.5f, 1f);
        barRoot.anchorMax = new Vector2(0.5f, 1f);
        barRoot.pivot = new Vector2(0.5f, 1f);
        barRoot.anchoredPosition = new Vector2(0, -6);
        barRoot.sizeDelta = new Vector2(0, 44);

        GameObject tt = new GameObject("RelicTooltip", typeof(RectTransform), typeof(Image));
        tooltipPanel = tt;
        tooltipRt = tt.GetComponent<RectTransform>();
        tt.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.96f);
        tooltipRt.SetParent(canvasGo.transform, false);
        tooltipRt.anchorMin = new Vector2(0.5f, 1f);
        tooltipRt.anchorMax = new Vector2(0.5f, 1f);
        tooltipRt.pivot = new Vector2(0.5f, 0f);
        tooltipRt.sizeDelta = new Vector2(340, 100);
        tt.SetActive(false);

        tooltipName = RuntimeUi.CreateText(tt.transform, "", 20, TextAnchor.MiddleLeft, Vector2.zero, Vector2.one);
        tooltipName.fontStyle = FontStyle.Bold;
        tooltipName.color = new Color(0.95f, 0.9f, 0.4f);
        tooltipName.rectTransform.offsetMin = new Vector2(12, 52);
        tooltipName.rectTransform.offsetMax = new Vector2(-12, -8);

        tooltipDesc = RuntimeUi.CreateText(tt.transform, "", 16, TextAnchor.UpperLeft, Vector2.zero, Vector2.one);
        tooltipDesc.color = Color.white;
        tooltipDesc.rectTransform.offsetMin = new Vector2(12, 8);
        tooltipDesc.rectTransform.offsetMax = new Vector2(-12, 54);
    }

    public void Refresh()
    {
        if (barRoot == null)
            return;

        for (int i = barRoot.childCount - 1; i >= 0; i--)
            Destroy(barRoot.GetChild(i).gameObject);

        if (RelicManager.Instance == null)
            return;

        List<RelicData> relics = RelicManager.Instance.GetOwnedRelics();
        if (relics == null)
            return;

        float x = 0f;
        foreach (RelicData relic in relics)
        {
            CreateIcon(relic, x);
            x += 46f;
        }
    }

    private void CreateIcon(RelicData relic, float x)
    {
        GameObject go = new GameObject("RelicIcon", typeof(RectTransform), typeof(Image));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(barRoot, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(x, 0);
        rt.sizeDelta = new Vector2(40, 40);

        Image img = go.GetComponent<Image>();
        img.sprite = relic.icon;
        img.color = Color.white;

        RelicIconHover hover = go.AddComponent<RelicIconHover>();
        hover.Init(this, relic, rt);
    }

    private void OnRelicAdded(RelicData relic)
    {
        Refresh();
    }

    public void ShowTooltip(RelicData relic, RectTransform icon)
    {
        if (tooltipPanel == null || relic == null)
            return;

        tooltipName.text = relic.relicName;
        tooltipDesc.text = relic.description;
        tooltipPanel.SetActive(true);
        tooltipRt.position = icon.position + new Vector3(0, 26, 0);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }
}
