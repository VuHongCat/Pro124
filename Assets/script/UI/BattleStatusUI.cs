using UnityEngine;
using UnityEngine.UI;

public class BattleStatusUI : MonoBehaviour
{
    public static BattleStatusUI Instance;

    private Text statusText;
    private TurnManager turnManager;
    private DeckManager deckManager;

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

    private void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        deckManager = FindAnyObjectByType<DeckManager>();
        Refresh();
    }

    private void OnDestroy()
    {
        TurnManager.PlayerTurnStarted -= OnTurnStarted;
        DeckManager.PilesChanged -= OnPilesChanged;

        if (Instance == this)
            Instance = null;
    }

    private void BuildUI()
    {
        Canvas canvas = RuntimeUi.CreateCanvas("BattleStatusCanvas");
        canvas.sortingOrder = 200;

        GameObject panelGo = new GameObject("StatusPanel", typeof(RectTransform), typeof(Image));
        RectTransform panelRt = panelGo.GetComponent<RectTransform>();
        panelRt.SetParent(canvas.transform, false);
        panelRt.anchorMin = new Vector2(0, 1);
        panelRt.anchorMax = new Vector2(0, 1);
        panelRt.pivot = new Vector2(0, 1);
        panelRt.anchoredPosition = new Vector2(8, -8);
        panelRt.sizeDelta = new Vector2(360, 34);
        panelGo.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.85f);

        statusText = RuntimeUi.CreateText(panelRt, "", 18, TextAnchor.MiddleLeft, Vector2.zero, Vector2.one);
        statusText.rectTransform.offsetMin = new Vector2(10, 0);
        statusText.rectTransform.offsetMax = new Vector2(-10, 0);

        TurnManager.PlayerTurnStarted += OnTurnStarted;
        DeckManager.PilesChanged += OnPilesChanged;
    }

    private void OnTurnStarted(int turn)
    {
        Refresh();
    }

    private void OnPilesChanged()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (statusText == null) return;

        int turn = turnManager != null ? turnManager.TurnCount : 1;
        int deck = deckManager != null ? deckManager.DrawPileCount : 0;
        int used = deckManager != null ? deckManager.DiscardPileCount : 0;

        statusText.text = $"Turn {turn}  |  Deck {deck}  |  Used {used}";
    }
}
