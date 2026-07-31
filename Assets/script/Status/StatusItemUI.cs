using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text stackText;

    public void Setup(string name, Sprite icon, int stacks)
    {
        if (icon != null && iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false;
            Debug.LogWarning($"StatusItem '{name}': BuffIcon trống, hiện tên thay thế.");
        }

        if (stackText != null)
        {
            stackText.text = icon != null ? stacks.ToString() : $"{name} x{stacks}";
        }
    }
}
