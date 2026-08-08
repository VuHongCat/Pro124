using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CardGridUi
{
    public static readonly Color AttackFrame = new Color(0.6f, 0.22f, 0.22f, 1f);
    public static readonly Color BlockFrame = new Color(0.2f, 0.38f, 0.68f, 1f);
    public static readonly Color HealFrame = new Color(0.2f, 0.52f, 0.32f, 1f);
    public static readonly Color DarkFrame = new Color(0.12f, 0.12f, 0.16f, 1f);

    private static Sprite placeholder;

    public static List<CardData> UniqueCards(List<CardData> deck)
    {
        List<CardData> unique = new List<CardData>();
        if (deck == null) return unique;

        HashSet<string> seen = new HashSet<string>();
        foreach (CardData card in deck)
        {
            if (card == null) continue;
            if (!seen.Add(card.cardName)) continue;
            unique.Add(card);
        }

        return unique;
    }

    public static bool AllUpgraded(List<CardData> deck, string cardName)
    {
        if (deck == null) return false;

        foreach (CardData card in deck)
        {
            if (card == null) continue;
            if (card.cardName != cardName) continue;
            if (!card.isUpgraded) return false;
        }

        return true;
    }

    public static GameObject CreateCell(Transform parent, CardData card, Vector2 anchoredPos, bool dim, Action<CardData> onClick)
    {
        GameObject cell = new GameObject("CardCell", typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rt = cell.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(190, 220);

        Image frame = cell.GetComponent<Image>();
        frame.color = dim ? DarkFrame : GetFrameColor(card);

        Button btn = cell.GetComponent<Button>();
        btn.targetGraphic = frame;
        btn.interactable = !dim;
        if (!dim && onClick != null)
            btn.onClick.AddListener(() => onClick(card));

        GameObject artGo = new GameObject("Art", typeof(RectTransform), typeof(Image));
        RectTransform artRt = artGo.GetComponent<RectTransform>();
        artRt.SetParent(rt, false);
        artRt.anchorMin = new Vector2(0.5f, 1f);
        artRt.anchorMax = new Vector2(0.5f, 1f);
        artRt.pivot = new Vector2(0.5f, 1f);
        artRt.anchoredPosition = new Vector2(0, -6);
        artRt.sizeDelta = new Vector2(178, 150);
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
        if (dim) art.color *= new Color(0.55f, 0.55f, 0.55f, 0.7f);

        CreateManaBadge(rt, card.energyCost, dim);

        GameObject nameBar = new GameObject("NameBar", typeof(RectTransform), typeof(Image));
        RectTransform nameRt = nameBar.GetComponent<RectTransform>();
        nameRt.SetParent(rt, false);
        nameRt.anchorMin = new Vector2(0, 0);
        nameRt.anchorMax = new Vector2(1, 0.16f);
        nameRt.offsetMin = Vector2.zero;
        nameRt.offsetMax = Vector2.zero;
        nameBar.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.14f, 1f);

        Text name = RuntimeUi.CreateText(nameRt, card.isUpgraded ? card.cardName + " (+)" : card.cardName, 13, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        name.rectTransform.anchorMin = Vector2.zero;
        name.rectTransform.anchorMax = Vector2.one;
        name.rectTransform.offsetMin = new Vector2(2, 0);
        name.rectTransform.offsetMax = new Vector2(-2, 0);
        name.horizontalOverflow = HorizontalWrapMode.Wrap;
        if (card.isUpgraded) name.color = Color.cyan;

        return cell;
    }

    private static void CreateManaBadge(RectTransform parent, int cost, bool dim)
    {
        GameObject badge = new GameObject("Mana", typeof(RectTransform), typeof(Image));
        RectTransform rt = badge.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(18, -16);
        rt.sizeDelta = new Vector2(30, 30);
        Image bg = badge.GetComponent<Image>();
        bg.color = dim ? new Color(0.6f, 0.55f, 0.2f, 1f) : new Color(0.9f, 0.8f, 0.2f, 1f);

        Text costText = RuntimeUi.CreateText(rt, cost.ToString(), 15, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one);
        costText.color = Color.black;
    }

    public static Color GetFrameColor(CardData card)
    {
        switch (card.cardType)
        {
            case CardType.Attack: return AttackFrame;
            case CardType.Block: return BlockFrame;
            case CardType.Heal: return HealFrame;
            default: return DarkFrame;
        }
    }

    private static Sprite GetPlaceholder()
    {
        if (placeholder == null)
        {
            var tex = new Texture2D(2, 2);
            tex.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            tex.Apply();
            placeholder = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
            placeholder.name = "CardGridPlaceholder";
        }

        return placeholder;
    }
}
