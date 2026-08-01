using UnityEngine;
using TMPro;


public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;


    [Header("Tooltip Panel")]
    public GameObject panel;


    [Header("Text")]
    public TMP_Text nameText;
    public TMP_Text rarityText;
    public TMP_Text descriptionText;


    private RectTransform rectTransform;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }


        rectTransform = panel.GetComponent<RectTransform>();


        panel.SetActive(false);


        Debug.Log("TooltipUI Ready");
    }





    public void ShowRelic(RelicData relic)
    {
        panel.SetActive(true);



        // =====================
        // RELIC NAME
        // =====================

        nameText.text = relic.relicName;


        // Tên relic luôn màu xanh lá
        nameText.color =
            new Color(0.2f, 1f, 0.2f);





        // =====================
        // RELIC RARITY
        // =====================

        rarityText.text =
            relic.rarity.ToString();



        switch (relic.rarity)
        {
            // Common = xanh biển
            case RelicRarity.Common:

                rarityText.color =
                    new Color(0.2f, 0.6f, 1f);

                break;



            // Uncommon = đỏ
            case RelicRarity.Uncommon:

                rarityText.color =
                    new Color(1f, 0.2f, 0.2f);

                break;



            // Rare = vàng
            case RelicRarity.Rare:

                rarityText.color =
                    new Color(1f, 0.85f, 0f);

                break;



            // Boss = cam
            case RelicRarity.Boss:

                rarityText.color =
                    new Color(1f, 0.5f, 0f);

                break;
        }





        // =====================
        // RELIC EFFECT
        // =====================

        descriptionText.text =
            relic.description;



        UpdatePosition();



        Debug.Log(
            "Show tooltip: "
            + relic.relicName
        );
    }





    public void Hide()
    {
        panel.SetActive(false);
    }





    private void Update()
    {
        if (panel.activeSelf)
        {
            UpdatePosition();
        }
    }





    private void UpdatePosition()
    {
        rectTransform.position =
            Input.mousePosition
            + new Vector3(40, 20, 0);
    }
}