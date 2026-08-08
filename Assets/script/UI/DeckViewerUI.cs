using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewerUI : MonoBehaviour
{
    public static DeckViewerUI Instance;

    private GameObject panelRoot;
    private Text deckButtonLabel;
    private Text infoText;
    private Sprite placeholderSprite;
    private readonly List<GameObject> deckCells = new();

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
        Canvas canvas = RuntimeUi.CreateCanvas("DeckViewerCanvas");
        canvas.sortingOrder = 150;

        GameObject btnGo = new GameObject("DeckButton", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform btnRt = btnGo.GetComponent<RectTransform>();
        btnRt.SetParent(canvas.transform, false);
        btnRt.anchorMin = new Vector2(0, 0);
        btnRt.anchorMax = new Vector2(0, 0);
        btnRt.pivot = new Vector2(0, 0);
        btnRt.anchoredPosition = new Vector2(8, 8);
        btnRt.sizeDelta = new Vector2(180, 42);

        Image img = btnGo.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        Button btn = btnGo.GetComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(TogglePanel);

        deckButtonLabel = RuntimeUi.CreateText(btnRt, "Deck (0)", 16, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);

        panelRoot = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.92f));
        panelRoot.SetActive(false);

        RuntimeUi.CreateText(panelRoot.transform, "Your Deck", 24, TextAnchor.UpperCenter,
            new Vector2(0, 0.9f), new Vector2(1, 1));
        RuntimeUi.CreateButton(panelRoot.transform, "Close", new Vector2(0, -310), new Vector2(200, 50), TogglePanel);

        infoText = RuntimeUi.CreateText(panelRoot.transform, "", 16, TextAnchor.MiddleCenter,
            new Vector2(0, 0), new Vector2(1, 0.06f));
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

    private void EnsureDeck()
    {
        if (RunSession.Deck != null && RunSession.Deck.Count > 0)
            return;

        CardDatabase db = FindAnyObjectByType<CardDatabase>();
        if (db == null) return;

        RunSession.Deck = db.GetStarterDeck();
    }

    private void Refresh()
    {
        EnsureDeck();

        int count = RunSession.Deck != null ? RunSession.Deck.Count : 0;
        deckButtonLabel.text = $"Deck ({count})";

        foreach (GameObject cell in deckCells)
            Destroy(cell);
        deckCells.Clear();

        if (RunSession.Deck == null) return;

        const int cols = 5;
        const float cellW = 175f;
        const float cellH = 240f;
        int shown = 0;

        for (int i = 0; i < RunSession.Deck.Count; i++)
        {
            CardData card = RunSession.Deck[i];
            if (card == null) continue;
            if (shown >= 20) break;

            int row = shown / cols;
            int col = shown % cols;
            float x = (col - (cols - 1) * 0.5f) * cellW;
            float y = -85f - row * cellH;

            GameObject cell = new GameObject("CardCell", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform rt = cell.GetComponent<RectTransform>();
            rt.SetParent(panelRoot.transform, false);
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(155, 230);

            Image bg = cell.GetComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
            Button cellBtn = cell.GetComponent<Button>();
            cellBtn.targetGraphic = bg;
            CardData captured = card;
            cellBtn.onClick.AddListener(() => ShowInfo(captured));

            GameObject artGo = new GameObject("Art", typeof(RectTransform), typeof(Image));
            RectTransform artRt = artGo.GetComponent<RectTransform>();
            artRt.SetParent(rt, false);
            artRt.anchorMin = new Vector2(0.5f, 1f);
            artRt.anchorMax = new Vector2(0.5f, 1f);
            artRt.pivot = new Vector2(0.5f, 1f);
            artRt.anchoredPosition = new Vector2(0, -4);
            artRt.sizeDelta = new Vector2(145, 175);
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

            Text name = RuntimeUi.CreateText(rt, card.isUpgraded ? card.cardName + "+" : card.cardName, 13, TextAnchor.MiddleCenter,
                new Vector2(0, 0), new Vector2(1, 1));
            name.rectTransform.anchorMin = new Vector2(0, 0);
            name.rectTransform.anchorMax = new Vector2(1, 0.2f);
            name.rectTransform.offsetMin = new Vector2(2, 0);
            name.rectTransform.offsetMax = new Vector2(-2, 0);
            if (card.isUpgraded) name.color = Color.cyan;

            deckCells.Add(cell);
            shown++;
        }

        if (RunSession.Deck.Count > shown)
        {
            Text more = RuntimeUi.CreateText(panelRoot.transform, $"... and {RunSession.Deck.Count - shown} more", 14,
                TextAnchor.MiddleCenter, new Vector2(0, 0), new Vector2(1, 0.05f));
            deckCells.Add(more.gameObject);
        }
    }

    private void ShowInfo(CardData card)
    {
        if (infoText == null || card == null) return;
        infoText.text = $"{card.cardName} ({card.energyCost}E): {card.description}";
    }

    private Sprite GetPlaceholder()
    {
        if (placeholderSprite == null)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            placeholderSprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            placeholderSprite.name = "DeckArtPlaceholder";
        }

        return placeholderSprite;
    }
}
