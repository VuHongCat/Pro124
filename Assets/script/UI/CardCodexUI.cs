using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCodexUI : MonoBehaviour
{
    public static CardCodexUI Instance;

    private GameObject panelRoot;
    private Text codexButtonLabel;
    private Text infoText;
    private Sprite placeholderSprite;
    private readonly List<GameObject> cells = new();

    private CardType? currentFilter;
    private int page;
    private const int PageSize = 15;

    private static readonly Color AttackFrame = new Color(0.6f, 0.22f, 0.22f, 1f);
    private static readonly Color BlockFrame = new Color(0.2f, 0.38f, 0.68f, 1f);
    private static readonly Color HealFrame = new Color(0.2f, 0.52f, 0.32f, 1f);
    private static readonly Color CurseFrame = new Color(0.4f, 0.14f, 0.5f, 1f);
    private static readonly Color DarkFrame = new Color(0.12f, 0.12f, 0.16f, 1f);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void BuildUI()
    {
        Canvas canvas = RuntimeUi.CreateCanvas("CardCodexCanvas");
        canvas.sortingOrder = 160;

        GameObject btnGo = new GameObject("CodexButton", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform btnRt = btnGo.GetComponent<RectTransform>();
        btnRt.SetParent(canvas.transform, false);
        btnRt.anchorMin = new Vector2(0, 0);
        btnRt.anchorMax = new Vector2(0, 0);
        btnRt.pivot = new Vector2(0, 0);
        btnRt.anchoredPosition = new Vector2(8, 58);
        btnRt.sizeDelta = new Vector2(180, 42);

        Image img = btnGo.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        Button btn = btnGo.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(TogglePanel);

        codexButtonLabel = RuntimeUi.CreateText(btnRt, "Card Codex", 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

        panelRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.92f));
        panelRoot.SetActive(false);
    }

    private void TogglePanel()
    {
        if (panelRoot.activeSelf)
        {
            panelRoot.SetActive(false);
            return;
        }

        Refresh();
        panelRoot.SetActive(true);
    }

    private List<CardData> GetFilteredCards()
    {
        List<CardData> result = new();

        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        if (db != null)
        {
            foreach (CardData card in db.AllCards)
            {
                if (card == null) continue;
                if (currentFilter != null && card.cardType != currentFilter) continue;
                result.Add(card);
            }
        }

        foreach (CardData curse in CurseLibrary.GetCurses())
        {
            if (curse == null) continue;
            if (currentFilter != null && curse.cardType != currentFilter) continue;
            result.Add(curse);
        }

        result.Sort((a, b) =>
        {
            int type = a.cardType.CompareTo(b.cardType);
            if (type != 0) return type;
            int cost = a.energyCost.CompareTo(b.energyCost);
            if (cost != 0) return cost;
            return string.Compare(a.cardName, b.cardName, System.StringComparison.Ordinal);
        });

        return result;
    }

    private void Refresh()
    {
        if (panelRoot == null) return;

        foreach (Transform child in panelRoot.transform)
            Destroy(child.gameObject);
        cells.Clear();

        List<CardData> cards = GetFilteredCards();

        RuntimeUi.CreateText(panelRoot.transform, "Card Codex", 26, TextAnchor.UpperCenter,
            new Vector2(0, 0.9f), new Vector2(1, 1));

        if (codexButtonLabel != null)
            codexButtonLabel.text = $"Card Codex ({cards.Count})";

        CreateFilterButtons();

        int totalPages = Mathf.Max(1, Mathf.CeilToInt(cards.Count / (float)PageSize));
        page = Mathf.Clamp(page, 0, totalPages - 1);

        int start = page * PageSize;
        int end = Mathf.Min(start + PageSize, cards.Count);

        if (cards.Count == 0)
        {
            RuntimeUi.CreateText(panelRoot.transform, "No cards found", 20, TextAnchor.MiddleCenter,
                new Vector2(0, 0.55f), new Vector2(1, 0.7f));
        }
        else
        {
            BuildCells(cards, start, end);
        }

        CreatePageControls(totalPages);
        CreateCloseButton();

        infoText = RuntimeUi.CreateText(panelRoot.transform, "", 14, TextAnchor.MiddleCenter,
            new Vector2(0, 0), new Vector2(1, 0.09f));
    }

    private void CreateFilterButtons()
    {
        string[] labels = { "All", "Attack", "Block", "Heal", "Curse" };
        CardType?[] filters = { null, CardType.Attack, CardType.Block, CardType.Heal, CardType.Curse };

        for (int i = 0; i < labels.Length; i++)
        {
            CardType? filter = filters[i];
            bool active = currentFilter == filter;
            GameObject go = new GameObject("Filter_" + labels[i], typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(panelRoot.transform, false);
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(-248 + i * 124, -66);
            rt.sizeDelta = new Vector2(116, 40);
            Image img = go.GetComponent<Image>();
            img.color = active ? new Color(0.5f, 0.5f, 0.55f, 1f) : new Color(0.15f, 0.15f, 0.2f, 1f);
            Button btn = go.GetComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() =>
            {
                currentFilter = filter;
                page = 0;
                Refresh();
            });

            RuntimeUi.CreateText(rt, labels[i], 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        }
    }

    private void BuildCells(List<CardData> cards, int start, int end)
    {
        const int cols = 5;
        const float cellW = 175f;
        const float cellH = 200f;

        for (int i = start; i < end; i++)
        {
            CardData card = cards[i];
            if (card == null) continue;

            int index = i - start;
            int row = index / cols;
            int col = index % cols;
            float x = (col - (cols - 1) * 0.5f) * cellW;
            float y = -150f - row * cellH;

            GameObject cell = new GameObject("CardCell", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = cell.GetComponent<RectTransform>();
            rt.SetParent(panelRoot.transform, false);
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(155, 190);

            Image frame = cell.GetComponent<Image>();
            frame.color = GetFrameColor(card);
            Button cellBtn = cell.GetComponent<Button>();
            cellBtn.targetGraphic = frame;
            CardData captured = card;
            cellBtn.onClick.AddListener(() => ShowInfo(captured));

            CreateManaBadge(rt, card.energyCost);

            GameObject artGo = new GameObject("Art", typeof(RectTransform), typeof(Image));
            RectTransform artRt = artGo.GetComponent<RectTransform>();
            artRt.SetParent(rt, false);
            artRt.anchorMin = new Vector2(0.5f, 1f);
            artRt.anchorMax = new Vector2(0.5f, 1f);
            artRt.pivot = new Vector2(0.5f, 1f);
            artRt.anchoredPosition = new Vector2(0, -6);
            artRt.sizeDelta = new Vector2(143, 140);
            Image art = artGo.GetComponent<Image>();
            if (card.artwork != null)
            {
                art.sprite = card.artwork;
                art.color = Color.white;
            }
            else
            {
                art.sprite = GetPlaceholder();
                art.color = new Color(0.25f, 0.25f, 0.3f, 1f);
            }

            GameObject nameBar = new GameObject("NameBar", typeof(RectTransform), typeof(Image));
            RectTransform nameRt = nameBar.GetComponent<RectTransform>();
            nameRt.SetParent(rt, false);
            nameRt.anchorMin = new Vector2(0, 0);
            nameRt.anchorMax = new Vector2(1, 0.16f);
            nameRt.offsetMin = Vector2.zero;
            nameRt.offsetMax = Vector2.zero;
            nameBar.GetComponent<Image>().color = DarkFrame;

            Text name = RuntimeUi.CreateText(nameRt, card.cardName, 12, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            name.rectTransform.anchorMin = Vector2.zero;
            name.rectTransform.anchorMax = Vector2.one;
            name.rectTransform.offsetMin = new Vector2(2, 0);
            name.rectTransform.offsetMax = new Vector2(-2, 0);
            name.horizontalOverflow = HorizontalWrapMode.Wrap;

            cells.Add(cell);
        }
    }

    private void CreateManaBadge(RectTransform parent, int cost)
    {
        GameObject badge = new GameObject("Mana", typeof(RectTransform), typeof(Image));
        RectTransform rt = badge.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(16, -14);
        rt.sizeDelta = new Vector2(30, 30);
        Image bg = badge.GetComponent<Image>();
        bg.color = new Color(0.9f, 0.8f, 0.2f, 1f);

        Text costText = RuntimeUi.CreateText(rt, cost.ToString(), 15, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        costText.color = Color.black;
    }

    private void CreatePageControls(int totalPages)
    {
        Text pageText = RuntimeUi.CreateText(panelRoot.transform, $"Page {page + 1} / {totalPages}", 16, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        RectTransform pt = pageText.rectTransform;
        pt.anchorMin = new Vector2(0.5f, 1f);
        pt.anchorMax = new Vector2(0.5f, 1f);
        pt.pivot = new Vector2(0.5f, 1f);
        pt.anchoredPosition = new Vector2(0, -800);
        pt.sizeDelta = new Vector2(200, 40);

        bool hasPrev = page > 0;
        bool hasNext = page < totalPages - 1;

        CreateTopButton("Prev", new Vector2(-160, -800), new Vector2(120, 42), () => { page--; Refresh(); }, hasPrev);
        CreateTopButton("Next", new Vector2(160, -800), new Vector2(120, 42), () => { page++; Refresh(); }, hasNext);
    }

    private void CreateCloseButton()
    {
        CreateTopButton("Close", new Vector2(310, -800), new Vector2(140, 42), TogglePanel, true);
    }

    private void CreateTopButton(string label, Vector2 pos, Vector2 size, System.Action onClick, bool interactable)
    {
        GameObject go = new GameObject("Btn_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(panelRoot.transform, false);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.interactable = interactable;
        if (interactable)
            btn.onClick.AddListener(() => onClick());
        if (!interactable)
        {
            ColorBlock cb = btn.colors;
            cb.disabledColor = new Color(0.45f, 0.45f, 0.5f, 0.7f);
            btn.colors = cb;
        }

        RuntimeUi.CreateText(rt, label, 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
    }

    private void ShowInfo(CardData card)
    {
        if (infoText == null || card == null) return;

        CardData up = Instantiate(card);
        up.Upgrade();

        string rarityName = card.rarity.ToString();
        string baseDesc = card.description;
        string upDesc = up.description;

        infoText.text = $"{card.cardName} ({card.energyCost} Energy) | {card.cardType} | {rarityName}" +
            $"\nBase: {baseDesc}" +
            $"\nUpgrade: {upDesc}" +
            (upDesc == baseDesc ? "\n(Chỉ số được cải thiện, mô tả không đổi)" : "");
        infoText.color = Color.white;
    }

    private Color GetFrameColor(CardData card)
    {
        switch (card.cardType)
        {
            case CardType.Attack: return AttackFrame;
            case CardType.Block: return BlockFrame;
            case CardType.Heal: return HealFrame;
            case CardType.Curse: return CurseFrame;
            default: return DarkFrame;
        }
    }

    private Sprite GetPlaceholder()
    {
        if (placeholderSprite == null)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            placeholderSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            placeholderSprite.name = "CodexArtPlaceholder";
        }

        return placeholderSprite;
    }
}
