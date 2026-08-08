using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Cards")]
    public CardData[] allCards;

    public CardTooltipTrigger[] cardSlots;

    public TMP_Text[] cardPriceTexts;

    [Header("Relics")]
    public RelicData[] allRelics;

    public RelicTooltipTrigger[] relicSlots;

    public TMP_Text[] relicPriceTexts;

    private CardData[] currentCards;

    private RelicData[] currentRelics;

    private bool[] relicSold;

    private bool[] cardSold;

    private const int UpgradeCost = 75;
    private const int UpgradePageSize = 8;
    private int upgradePage;
    private GameObject upgradePanel;
    private Text upgradeMessage;

    private void Start()
    {
        GenerateCards();
        GenerateRelics();
        WireLeaveButton();
        WireCardBuyButtons();
        WireRelicBuyButtons();
        UpdateGoldText();
        CreateUpgradeButton();
    }

    //==========================
    // LEAVE
    //==========================

    public void Leave()
    {
        string shopNode = PlayerPrefs.GetString(
            MapManager.BattleNodeKey,
            ""
        );

        if (!string.IsNullOrEmpty(shopNode))
            MapManager.SaveCompletedNode(shopNode);

        RunSession.ReturnToMap();
    }

    private void WireLeaveButton()
    {
        Button leaveBtn = GameObject.Find("LeaveBotton")?.GetComponent<Button>();

        if (leaveBtn == null)
        {
            Debug.LogWarning("[ShopManager] LeaveBotton button not found!");
            return;
        }

        leaveBtn.onClick.RemoveAllListeners();
        leaveBtn.onClick.AddListener(Leave);
    }

    private void UpdateGoldText()
    {
        TMP_Text goldText = GameObject.Find("GoldText")?.GetComponent<TMP_Text>();

        if (goldText != null)
            goldText.text = RunSession.Gold.ToString();
    }

    //==========================
    // BUY RELIC
    //==========================

    private void WireRelicBuyButtons()
    {
        relicSold = new bool[relicSlots.Length];

        for (int i = 0; i < relicSlots.Length; i++)
        {
            if (relicSlots[i] == null)
                continue;

            Button btn = relicSlots[i].GetComponent<Button>();

            if (btn == null)
                continue;

            int index = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => BuyRelic(index));
        }
    }

    public void BuyRelic(int index)
    {
        if (index < 0 || index >= relicSlots.Length)
            return;

        if (relicSold[index])
            return;

        RelicData relic = currentRelics[index];

        if (relic == null)
            return;

        if (RunSession.Gold < relic.shopPrice)
        {
            Debug.Log("[ShopManager] Not enough gold to buy relic: " + relic.relicName);
            return;
        }

        RunSession.Gold -= relic.shopPrice;

        RelicManager.Instance.AddRelic(relic);

        relicSold[index] = true;

        Button btn = relicSlots[index].GetComponent<Button>();
        if (btn != null)
            btn.interactable = false;

        if (relicPriceTexts[index] != null)
            relicPriceTexts[index].text = "SOLD";

        UpdateGoldText();
    }

    //==========================
    // CARD
    //==========================

    void GenerateCards()
    {
        List<CardData> availableCards =
            new List<CardData>(allCards);

        currentCards =
            new CardData[cardSlots.Length];

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (availableCards.Count == 0)
                break;

            int randomIndex =
                Random.Range(0, availableCards.Count);

            CardData card =
                availableCards[randomIndex];

            currentCards[i] = card;

            // Gán card vào slot
            cardSlots[i].SetCard(card);

            // Hiện giá
            cardPriceTexts[i].text =
                card.shopPrice + " Gold";

            // Tránh trùng
            availableCards.RemoveAt(randomIndex);
        }
    }

    private void WireCardBuyButtons()
    {
        cardSold = new bool[cardSlots.Length];

        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (cardSlots[i] == null)
                continue;

            Button btn = cardSlots[i].GetComponent<Button>();
            if (btn == null)
                btn = cardSlots[i].gameObject.AddComponent<Button>();

            int index = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => BuyCard(index));
        }
    }

    public void BuyCard(int index)
    {
        if (index < 0 || index >= cardSlots.Length)
            return;

        if (cardSold[index])
            return;

        CardData card = currentCards[index];
        if (card == null)
            return;

        if (RunSession.Gold < card.shopPrice)
        {
            Debug.Log("[ShopManager] Not enough gold to buy card: " + card.cardName);
            return;
        }

        RunSession.Gold -= card.shopPrice;
        RunSession.Deck.Add(card);
        cardSold[index] = true;

        Button btn = cardSlots[index].GetComponent<Button>();
        if (btn != null)
            btn.interactable = false;

        if (cardPriceTexts[index] != null)
            cardPriceTexts[index].text = "SOLD";

        UpdateGoldText();
    }

    //==========================
    // RELIC
    //==========================

    void GenerateRelics()
    {
        List<RelicData> availableRelics =
            new List<RelicData>();

        foreach (RelicData relic in allRelics)
        {
            if (relic.rarity != RelicRarity.Boss)
            {
                if (!relic.stackable && RelicManager.Instance.HasRelic(relic.relicName))
                    continue;

                availableRelics.Add(relic);
            }
        }

        currentRelics =
            new RelicData[relicSlots.Length];

        for (int i = 0; i < relicSlots.Length; i++)
        {
            if (availableRelics.Count == 0)
                break;

            int randomIndex =
                Random.Range(0, availableRelics.Count);

            RelicData relic =
                availableRelics[randomIndex];

            currentRelics[i] = relic;

            // Gán relic
            relicSlots[i].SetRelic(relic);

            // Giá
            relicPriceTexts[i].text =
                relic.shopPrice + " Gold";

            // Tránh trùng
            availableRelics.RemoveAt(randomIndex);
        }
    }

    //==========================
    // GET DATA
    //==========================

    public CardData GetCard(int index)
    {
        return currentCards[index];
    }

    public RelicData GetRelic(int index)
    {
        return currentRelics[index];
    }

    //==========================
    // REFRESH SHOP
    //==========================

    public void RefreshShop()
    {
        GenerateCards();
        GenerateRelics();
    }

    //==========================
    // UPGRADE CARD
    //==========================

    private void CreateUpgradeButton()
    {
        Button leaveBtn = GameObject.Find("LeaveBotton")?.GetComponent<Button>();
        Transform parent = leaveBtn != null ? leaveBtn.transform.parent : null;

        if (parent != null)
        {
            GameObject go = new GameObject("UpgradeButton", typeof(RectTransform), typeof(Image));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(560, -451.5f);
            rt.sizeDelta = new Vector2(210, 51);

            Image img = go.GetComponent<Image>();
            img.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
            Button btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(OpenUpgradePanel);
            RuntimeUi.CreateText(go.transform, $"Upgrade Card ({UpgradeCost} Gold)", 18, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
            return;
        }

        Canvas canvas = RuntimeUi.CreateCanvas("UpgradeCanvas");
        RuntimeUi.CreateButton(canvas.transform, $"Upgrade Card ({UpgradeCost} Gold)",
            new Vector2(230, 300), new Vector2(250, 48), OpenUpgradePanel);
    }

    private void OpenUpgradePanel()
    {
        if (upgradePanel != null) return;
        upgradePage = 0;

        Canvas canvas = RuntimeUi.CreateCanvas("ShopUpgradeCanvas");
        upgradePanel = RuntimeUi.CreatePanel(canvas.transform, new Color(0, 0, 0, 0.85f));
        ShowUpgradePage();
    }

    private void ShowUpgradePage()
    {
        if (upgradePanel == null) return;

        for (int i = upgradePanel.transform.childCount - 1; i >= 0; i--)
            Destroy(upgradePanel.transform.GetChild(i).gameObject);

        List<CardData> deck = CardGridUi.UniqueCards(RunSession.Deck);
        if (deck.Count == 0)
        {
            RuntimeUi.CreateText(upgradePanel.transform, "Deck is empty!", 22, TextAnchor.MiddleCenter,
                new Vector2(0, 0.55f), new Vector2(1, 0.7f));
            RuntimeUi.CreateButton(upgradePanel.transform, "Close", new Vector2(0, -180), new Vector2(200, 55), CloseUpgradePanel);
            return;
        }

        int totalPages = Mathf.Max(1, Mathf.CeilToInt(deck.Count / (float)UpgradePageSize));
        upgradePage = Mathf.Clamp(upgradePage, 0, totalPages - 1);

        RuntimeUi.CreateText(upgradePanel.transform,
            $"Upgrade Card - {UpgradeCost} Gold (page {upgradePage + 1} / {totalPages})", 24, TextAnchor.MiddleCenter,
            new Vector2(0, 0.82f), new Vector2(1, 0.96f));

        int start = upgradePage * UpgradePageSize;
        int end = Mathf.Min(start + UpgradePageSize, deck.Count);

        const int cols = 4;
        const float cellW = 200f;
        const float cellH = 240f;

        for (int i = start; i < end; i++)
        {
            CardData card = deck[i];
            if (card == null) continue;

            int index = i - start;
            int row = index / cols;
            int col = index % cols;
            float x = (col - (cols - 1) * 0.5f) * cellW;
            float y = -80f - row * cellH;

            bool dim = CardGridUi.AllUpgraded(RunSession.Deck, card.cardName);
            CardData captured = card;
            CardGridUi.CreateCell(upgradePanel.transform, card, new Vector2(x, y), dim,
                c => TryUpgrade(captured));
        }

        RuntimeUi.CreateButton(upgradePanel.transform, "Previous", new Vector2(-170, -480), new Vector2(140, 46),
            () => { upgradePage--; ShowUpgradePage(); }, upgradePage > 0);

        RuntimeUi.CreateButton(upgradePanel.transform, "Next", new Vector2(170, -480), new Vector2(140, 46),
            () => { upgradePage++; ShowUpgradePage(); }, end < deck.Count);

        upgradeMessage = RuntimeUi.CreateText(upgradePanel.transform, "", 20, TextAnchor.MiddleCenter,
            new Vector2(0.2f, -0.36f), new Vector2(0.8f, -0.3f));
        upgradeMessage.color = new Color(1f, 0.35f, 0.35f);

        RuntimeUi.CreateButton(upgradePanel.transform, "Close", new Vector2(0, -540), new Vector2(200, 48), CloseUpgradePanel);
    }

    private void TryUpgrade(CardData card)
    {
        if (card == null || card.isUpgraded) return;

        if (RunSession.Gold < UpgradeCost)
        {
            Debug.Log("[ShopManager] Not enough gold to upgrade: " + card.cardName);
            if (upgradeMessage != null)
                upgradeMessage.text = "Not enough gold! Need " + UpgradeCost + " Gold";
            return;
        }

        RunSession.Gold -= UpgradeCost;
        RunSession.UpgradeCards(card.cardName);
        RelicManager.EmitRestSite();
        UpdateGoldText();
        ShowUpgradePage();
        if (upgradeMessage != null)
            upgradeMessage.text = "Upgraded: " + card.cardName + "!";
        Debug.Log("[ShopManager] Upgraded: " + card.cardName);
    }

    private void CloseUpgradePanel()
    {
        if (upgradePanel != null)
        {
            Destroy(upgradePanel);
            upgradePanel = null;
        }
    }
}