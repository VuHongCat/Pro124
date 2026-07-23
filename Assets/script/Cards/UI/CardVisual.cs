using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotateSpeed = 12f;
    [SerializeField] private float scaleSpeed = 12f;

    private RectTransform rect;

    private Vector2 targetPosition;
    private Quaternion targetRotation;
    private Vector3 targetScale;

    public bool IsDragging { get; set; }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        targetPosition = rect.anchoredPosition;
        targetRotation = rect.localRotation;
        targetScale = rect.localScale;
    }

    private void Update()
    {
        if (IsDragging) return;
        rect.anchoredPosition = Vector2.Lerp(
            rect.anchoredPosition,
            targetPosition,
            Time.deltaTime * moveSpeed);

        rect.localRotation = Quaternion.Lerp(
            rect.localRotation,
            targetRotation,
            Time.deltaTime * rotateSpeed);

        rect.localScale = Vector3.Lerp(
            rect.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed);
    }

    public void SetTarget(Vector2 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }

    public void SetScale(Vector3 scale)
    {
        targetScale = scale;
    }
}
