using System.Collections.Generic;
using UnityEngine;

public class HandLayout : MonoBehaviour
{
    [SerializeField] private float spacing = 220f;

    public void UpdateLayout(List<CardDisplay> cards)
    {
        int cardCount = cards.Count;

        if (cardCount == 0) return;

        float startX = -(cardCount - 1) * spacing * 0.5f;
        float center = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            CardDisplay display = cards[i];
            if (display == null) continue;

            RectTransform card = display.transform as RectTransform;

            float curveHeight = 25f;

            float x = startX + i * spacing;

            float distance = Mathf.Abs(i - center);

            float y = -(distance * curveHeight);

            float angle = (i - center) * -5f;
            CardVisual visual = display.GetComponent<CardVisual>();

            visual.SetTarget(new Vector2(x, y), Quaternion.Euler(0, 0, angle));
        }
    }
}
