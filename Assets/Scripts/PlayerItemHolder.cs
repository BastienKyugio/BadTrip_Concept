using Unity.Netcode;
using UnityEngine;

public class PlayerItemHolder : NetworkBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private string heldItem = "";

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
                interactable.Interact(gameObject);
            }
        }
    }

    public void GiveItem(string itemName)
    {
        heldItem = itemName;
        Debug.Log($"{OwnerClientId} a ramassé : {itemName}");
        // Plus tard : afficher l’objet dans la main ou ajouter à un inventaire
    }
}
