using UnityEngine;
using UnityEngine.EventSystems;

public class CardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverHeight = 30f;
    [SerializeField] private float hoverScale = 1.2f;

    private Vector3 originalPos;
    private Vector3 originalScale;

    private RectTransform rect;
    private CardVisual visual;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        visual = GetComponent<CardVisual>();
        originalScale = rect.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        originalPos = rect.localPosition;
        rect.localPosition += Vector3.up * hoverHeight;
        visual.SetScale(originalScale * hoverScale);
        rect.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.localPosition = originalPos;

        visual.SetScale(originalScale);
    }
}
