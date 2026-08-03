using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    [Header("Cards")]
    public CardData[] allCards;

    public Image[] cardSlots;

    public TMP_Text[] cardPriceTexts;



    [Header("Relics")]
    public RelicData[] allRelics;

    public RelicTooltipTrigger[] relicSlots;

    public TMP_Text[] relicPriceTexts;



    private CardData[] currentCards;

    private RelicData[] currentRelics;





    private void Start()
    {
        GenerateCards();
        GenerateRelics();
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
                Random.Range(
                    0,
                    availableCards.Count
                );



            CardData card =
                availableCards[randomIndex];



            currentCards[i] = card;



            // Hiện hình card
            cardSlots[i].sprite =
                card.artwork;



            // Hiện giá card
            cardPriceTexts[i].text =
                card.shopPrice + " Gold";



            // Tránh trùng card
            availableCards.RemoveAt(randomIndex);
        }
    }







    //==========================
    // RELIC
    //==========================

    void GenerateRelics()
    {
        List<RelicData> availableRelics =
            new List<RelicData>();



        // Không cho Boss Relic xuất hiện trong shop
        foreach (RelicData relic in allRelics)
        {
            if (relic.rarity != RelicRarity.Boss)
            {
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
                Random.Range(
                    0,
                    availableRelics.Count
                );



            RelicData relic =
                availableRelics[randomIndex];



            currentRelics[i] = relic;



            // Gán relic vào slot
            relicSlots[i].SetRelic(relic);



            // Hiện giá relic
            relicPriceTexts[i].text =
                relic.shopPrice + " Gold";



            // Tránh trùng relic
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