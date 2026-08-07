using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text stackText;

    private string statusName;
    private string statusDescription;

    public void Setup(string name, Sprite icon, int stacks, string description = null)
    {
        statusName = name;
        statusDescription = description;

        if (icon != null && iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
            Debug.LogWarning($"StatusItem '{name}': BuffIcon is empty, showing name instead.");
        }

        if (stackText != null)
        {
            stackText.text = icon != null ? stacks.ToString() : $"{name} x{stacks}";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StatusTooltip.Show($"{statusName} ({stackText?.text})", statusDescription, (RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StatusTooltip.Hide();
    }

    private void Update()
    {
        StatusTooltip.Tick();
    }

    private void OnDisable()
    {
        StatusTooltip.Hide();
    }
}
