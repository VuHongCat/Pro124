using System.Collections.Generic;
using UnityEngine;

public class StatusHolderUI : MonoBehaviour
{
    [SerializeField] private GameObject statusItemPrefab;
    [SerializeField] private Transform container;

    private readonly Dictionary<string, StatusItemUI> items = new();

    public void SetStatus(string id, string name, Sprite icon, int stacks, string description = null)
    {
        if (statusItemPrefab == null)
        {
            Debug.LogError($"StatusHolderUI on {name}: StatusItemPrefab not assigned!", this);
            return;
        }
        if (container == null)
        {
            Debug.LogError($"StatusHolderUI on {name}: Container not assigned!", this);
            return;
        }

        if (!items.TryGetValue(id, out StatusItemUI item))
        {
            GameObject go = Instantiate(statusItemPrefab, container);
            item = go.GetComponent<StatusItemUI>();
            if (item == null)
            {
                Debug.LogError($"Prefab '{statusItemPrefab.name}' has no StatusItemUI script!", this);
                Destroy(go);
                return;
            }
            items[id] = item;
        }
        item.Setup(name, icon, stacks, description);
    }

    public void RemoveStatus(string id)
    {
        if (items.TryGetValue(id, out StatusItemUI item))
        {
            Destroy(item.gameObject);
            items.Remove(id);
        }
    }
}
