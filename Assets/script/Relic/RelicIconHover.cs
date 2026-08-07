using UnityEngine;
using UnityEngine.EventSystems;

public class RelicIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RelicBarUI bar;
    private RelicData relic;
    private RectTransform rt;

    public void Init(RelicBarUI bar, RelicData relic, RectTransform rt)
    {
        this.bar = bar;
        this.relic = relic;
        this.rt = rt;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bar.ShowTooltip(relic, rt);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bar.HideTooltip();
    }
}
