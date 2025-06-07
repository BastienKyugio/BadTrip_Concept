using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private KeyCode toggleKey = KeyCode.I;

    private Inventory playerInventory;

    public void SetInventory(Inventory inventory)
    {
        playerInventory = inventory;
        Debug.Log("👜 InventoryUI a reçu la référence à l'inventaire.");
    }

    private void Update()
    {
        if (playerInventory == null) return;

        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            RefreshInventory();
            inventoryPanel.SetActive(true);
        }
    }

    private void RefreshInventory()
    {
        if (playerInventory == null) return;

        string display = "Inventaire :\n";
        foreach (var item in playerInventory.Items)
        {
            display += "- " + item + "\n";
        }

        inventoryText.text = display;
    }
}
