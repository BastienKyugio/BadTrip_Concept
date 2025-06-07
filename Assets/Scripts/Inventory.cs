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
            Debug.Log($"[INVENTORY] Ajouté : {itemName}");
        }
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            Debug.Log($"[INVENTORY] Retiré : {itemName}");
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}
