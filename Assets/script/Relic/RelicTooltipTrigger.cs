using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RelicTooltipTrigger : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public RelicData relic;

    [Header("Relic Icon")]
    public Image icon;


    public void SetRelic(RelicData data)
    {
        relic = data;


        if (icon != null && relic != null)
        {
            icon.sprite = relic.icon;
        }


        Debug.Log(
            "Đã gán relic: " + relic.relicName
        );
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        if (relic == null)
        {
            Debug.LogWarning("RelicData NULL");
            return;
        }


        TooltipUI.Instance.ShowRelic(relic);
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Hide();
        }
    }
}