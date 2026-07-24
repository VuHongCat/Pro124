using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField] private BattleManager battleManager;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private CardVisual visual;

    private Vector2 originalPosition;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();

        visual = GetComponent<CardVisual>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        visual.IsDragging = true;

        originalPosition = rect.anchoredPosition;

        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        visual.IsDragging = false;

        canvasGroup.blocksRaycasts = true;

        rect.anchoredPosition = originalPosition;
    }
}