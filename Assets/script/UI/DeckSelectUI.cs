using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectUI : MonoBehaviour
{
    public static bool IsOpen;

    private GameObject panelRoot;
    private Action onFight;
    private Action onCancel;
    private readonly List<CardData> allCards = new();
    private readonly List<bool> selected = new();
    private readonly List<GameObject> rows = new();
    private Text countText;
    private int page;
    private const int MaxPageSize = 10;

    public static void Show(Action onConfirm, Action onCancel = null)
    {
        GameObject go = new GameObject("DeckSelectUI");
        DeckSelectUI ui = go.AddComponent<DeckSelectUI>();
        ui.Setup(onConfirm, onCancel);
    }

    private void Setup(Action onConfirmCallback, Action onCancelCallback)
    {
        onFight = onConfirmCallback;
        onCancel = onCancelCallback;
        IsOpen = true;

        RunSession.EnsureDeckReady();

        if (RunSession.Deck != null)
            allCards.AddRange(RunSession.Deck);

        selected.Clear();
        for (int i = 0; i < allCards.Count; i++)
            selected.Add(true);

        Canvas canvas = RuntimeUi.CreateCanvas("DeckSelectCanvas");
        canvas.sortingOrder = 160;

        panelRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.93f));

        RuntimeUi.CreateText(panelRoot.transform, "Build your deck", 26, TextAnchor.UpperCenter,
            new Vector2(0, 0.92f), new Vector2(1, 1));

        countText = RuntimeUi.CreateText(panelRoot.transform, "", 18, TextAnchor.UpperCenter,
            new Vector2(0, 0.85f), new Vector2(1, 0.92f));

        CreateBottomButton(panelRoot.transform, "Confirm", new Vector2(-110, 92), new Vector2(200, 50), Confirm);
        CreateBottomButton(panelRoot.transform, "Cancel", new Vector2(110, 92), new Vector2(200, 50), Close);

        CreateBottomButton(panelRoot.transform, "< Prev", new Vector2(-110, 40), new Vector2(160, 40), PrevPage);
        CreateBottomButton(panelRoot.transform, "Next >", new Vector2(110, 40), new Vector2(160, 40), NextPage);

        Render();
    }

    private Button CreateBottomButton(Transform parent, string label, Vector2 pos, Vector2 size, Action onClick)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);

        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => onClick());

        RuntimeUi.CreateText(go.transform, label, 18, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        return btn;
    }

    private void Render()
    {
        foreach (GameObject r in rows)
            Destroy(r);
        rows.Clear();

        int total = allCards.Count;
        int pages = Mathf.Max(1, Mathf.CeilToInt(total / (float)MaxPageSize));
        page = Mathf.Clamp(page, 0, pages - 1);

        int start = page * MaxPageSize;
        int end = Mathf.Min(start + MaxPageSize, total);

        for (int i = start; i < end; i++)
        {
            int index = i;
            CardData card = allCards[index];
            bool sel = selected[index];
            int local = index - start;

            GameObject row = new GameObject("Row", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = row.GetComponent<RectTransform>();
            rt.SetParent(panelRoot.transform, false);
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0, -115f - local * 36f);
            rt.sizeDelta = new Vector2(560, 34);

            Image bg = row.GetComponent<Image>();
            bg.color = sel
                ? new Color(0.15f, 0.45f, 0.2f, 1f)
                : new Color(0.32f, 0.32f, 0.38f, 1f);

            Button btn = row.GetComponent<Button>();
            btn.targetGraphic = bg;
            btn.onClick.AddListener(() => Toggle(index));

            RuntimeUi.CreateText(rt, (sel ? "[X] " : "[ ] ") + card.cardName + (card.isUpgraded ? " (Up)" : ""),
                16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

            rows.Add(row);
        }

        UpdateCount();
    }

    private void UpdateCount()
    {
        int count = 0;
        for (int i = 0; i < selected.Count; i++)
            if (selected[i])
                count++;

        int pages = Mathf.Max(1, Mathf.CeilToInt(allCards.Count / (float)MaxPageSize));
        countText.text = $"Selected: {count}/{allCards.Count}  (page {page + 1}/{pages}, max 30)";
    }

    private void Toggle(int index)
    {
        selected[index] = !selected[index];
        Render();
    }

    private void PrevPage()
    {
        page--;
        Render();
    }

    private void NextPage()
    {
        page++;
        Render();
    }

    private void Confirm()
    {
        if (this == null)
            return;

        List<CardData> battle = new();
        for (int i = 0; i < allCards.Count; i++)
            if (selected[i])
                battle.Add(allCards[i]);

        RunSession.BattleDeck = battle;
        RunSession.LastBuiltDeckCount = RunSession.Deck != null ? RunSession.Deck.Count : 0;
        Action cb = onFight;
        IsOpen = false;
        Destroy(gameObject);
        cb?.Invoke();
    }

    private void Close()
    {
        if (this == null)
            return;

        Action cb = onCancel;
        IsOpen = false;
        Destroy(gameObject);
        cb?.Invoke();
    }
}
