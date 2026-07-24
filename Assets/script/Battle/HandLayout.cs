using UnityEngine;

public class HandLayout : MonoBehaviour
{
    [SerializeField] private float spacing = 220f;

    public void UpdateLayout()
    {
        int cardCount = transform.childCount;

        if (cardCount == 0) return;

        float startX = -(cardCount - 1) * spacing * 0.5f;

        for(int i = 0; i < cardCount; i++)
        {
            RectTransform card = transform.GetChild(i).GetComponent<RectTransform>();

            float curveHeight = 25f;

            float x = startX + i * spacing;

            float center = (cardCount - 1) / 2f;

            float distance = Mathf.Abs(i - center);

            float y = -(distance * curveHeight);

            Vector2 targetPos = new Vector2(x, y);

            float angle = (i - center) * -5f;
            CardVisual visual = card.GetComponent<CardVisual>();

            visual.SetTarget(targetPos, Quaternion.Euler(0, 0, angle));
        }
    }
}
