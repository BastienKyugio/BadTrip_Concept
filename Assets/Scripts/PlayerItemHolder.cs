using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerItemHolder : NetworkBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private Inventory inventory;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log($"[PlayerItemHolder] NetworkObject ID = {NetworkObjectId}, IsOwner: {IsOwner}, IsClient: {IsClient}, IsServer: {IsServer}");
            Debug.Log($"[Player] Je suis le joueur local avec ClientId = {OwnerClientId}");
            inventory = GetComponent<Inventory>();

            // Relier à l’UI locale
            var inventoryUI = FindAnyObjectByType<InventoryUI>();
            if (inventoryUI != null)
            {
                inventoryUI.SetInventory(inventory);
                Debug.Log("👜 Inventory UI relié au joueur local.");
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (Physics.Raycast(interactionPoint.position, interactionPoint.forward, out RaycastHit hit, interactionRange, interactableLayer))
        {
            var interactable = hit.collider.GetComponent<InteractableItem>();
            if (interactable != null)
            {
                interactable.InteractServerRpc();
            }
        }
    }

    [ClientRpc]
    public void GiveItemToOwnerClientRpc(string itemName)
    {
        Debug.Log($"[CLIENT RPC] reçu GiveItemToOwnerClientRpc({itemName}) sur client {NetworkManager.Singleton.LocalClientId}");

        if (!IsOwner)
        {
            Debug.Log($"[CLIENT] Je ne suis pas Owner, donc j'ignore");
            return;
        }

        if (inventory == null)
            inventory = GetComponent<Inventory>();

        Debug.Log($"[ClientRpc] reçu sur client {NetworkManager.Singleton.LocalClientId}, ajout : {itemName}");
        inventory.AddItem(itemName);
    }

}
