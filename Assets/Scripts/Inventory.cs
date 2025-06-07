using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    public IReadOnlyList<string> Items => items;

    public void AddItem(string itemName)
    {
        if (!items.Contains(itemName))
        {
            items.Add(itemName);
            Debug.Log($"[INVENTORY] Ajout� : {itemName}");
        }
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            Debug.Log($"[INVENTORY] Retir� : {itemName}");
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}
