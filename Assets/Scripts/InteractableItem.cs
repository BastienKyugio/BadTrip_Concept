using Unity.Netcode;
using UnityEngine;

public class InteractableItem : NetworkBehaviour
{
    public string itemName = "Item";

    [SerializeField] private GameObject visual; // mesh à désactiver lors du ramassage

    private bool isTaken = false;

    public void Interact(GameObject player)
    {
        if (isTaken) return;

        // Si on veut limiter au joueur propriétaire
        if (!IsServer) return;

        isTaken = true;

        // Désactivation visuelle en réseau
        visual.SetActive(false);

        // Informer le joueur (tu pourras connecter à l’inventaire plus tard)
        var holder = player.GetComponent<PlayerItemHolder>();
        if (holder != null)
        {
            holder.GiveItem(itemName);
        }
    }
}
