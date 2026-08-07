using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handPanel;
    [SerializeField] private CardFactory cardFactory;
    [SerializeField] private HandLayout handLayout;
    [SerializeField] private int maxHandSize = 10;

    private List<CardDisplay> cardsInHand = new();
    public IReadOnlyList<CardDisplay> Cards => cardsInHand;
    public bool IsFull => cardsInHand.Count >= maxHandSize;

    public void AddCard(CardData data)
    {
        if (IsFull) return;
        GameObject cardObject = cardFactory.CreateCard(data, handPanel);
        CardDisplay display = cardObject.GetComponent<CardDisplay>();
        cardsInHand.Add(display);
        handLayout.UpdateLayout(cardsInHand);
    }

    public void AddCard(CardData data, int index)
    {
        if (IsFull) return;
        GameObject cardObject = cardFactory.CreateCard(data, handPanel);
        CardDisplay display = cardObject.GetComponent<CardDisplay>();
        int insertIndex = Mathf.Clamp(index, 0, cardsInHand.Count);
        cardsInHand.Insert(insertIndex, display);
        display.transform.SetSiblingIndex(insertIndex);
        handLayout.UpdateLayout(cardsInHand);
    }

    public int GetIndex(CardDisplay card) => cardsInHand.IndexOf(card);

    public void RemoveCard(CardDisplay card)
    {
        cardsInHand.Remove(card);
        StartCoroutine(FadeOutAndDestroy(card.gameObject));
        handLayout.UpdateLayout(cardsInHand);
    }

    private IEnumerator FadeOutAndDestroy(GameObject cardObject)
    {
        CanvasGroup group = cardObject.GetComponent<CanvasGroup>();
        if (group == null)
            group = cardObject.AddComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;

        float duration = 1f;
        Vector3 startScale = cardObject.transform.localScale;
        Vector3 endScale = startScale * 1.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            group.alpha = 1f - t;
            cardObject.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        Destroy(cardObject);
    }

    public void ClearHand()
    {
        foreach(CardDisplay card in cardsInHand)
        {
            Destroy(card.gameObject);
        }
        cardsInHand.Clear();
        handLayout.UpdateLayout(cardsInHand);
    }

    public List<CardDisplay> GetCardsInHand()
    {
        return new List<CardDisplay>(cardsInHand);
    }
}
