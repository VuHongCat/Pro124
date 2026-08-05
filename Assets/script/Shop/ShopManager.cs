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

    private void Start()
    {
        GenerateCards();
        GenerateRelics();
        WireLeaveButton();
        WireCardBuyButtons();
        WireRelicBuyButtons();
        UpdateGoldText();
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
}