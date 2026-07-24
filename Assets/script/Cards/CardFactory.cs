using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [Header("Card Prefab")]
    [SerializeField] private GameObject cardPrefab;

    public GameObject CreateCard(CardData data, Transform parent)
    {
        GameObject cardObject = Instantiate(cardPrefab, parent, false);
        CardDisplay display = cardObject.GetComponent<CardDisplay>();
        display.Setup(data);
        return cardObject;
    }
}
