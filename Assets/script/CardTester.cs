using UnityEngine;

public class CardTester : MonoBehaviour
{
    [SerializeField] private CardFactory factory;

    [SerializeField] private Transform hand;

    [SerializeField] private CardDatabase database;

    private void Start()
    {
        foreach (CardData card in database.AllCards)
        {
            factory.CreateCard(card, hand);
        }
    }
}
