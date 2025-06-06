using Unity.Netcode;
using UnityEngine;

public class InteractableItem : NetworkBehaviour
{
    public string itemName = "Item";

    [SerializeField] private GameObject visual; // mesh � d�sactiver lors du ramassage

    private bool isTaken = false;

    public void Interact(GameObject player)
    {
        if (isTaken) return;

        // Si on veut limiter au joueur propri�taire
        if (!IsServer) return;

        isTaken = true;

        // D�sactivation visuelle en r�seau
        visual.SetActive(false);

        // Informer le joueur (tu pourras connecter � l�inventaire plus tard)
        var holder = player.GetComponent<PlayerItemHolder>();
        if (holder != null)
        {
            holder.GiveItem(itemName);
        }
    }
}
