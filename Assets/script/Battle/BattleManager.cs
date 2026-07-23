using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private CardDatabase database;
    [SerializeField] private HandManager handManager;

    private void Start()
    {
        foreach(CardData card in database.AllCards)
        {
            handManager.AddCard(card);
        }
    }
}
