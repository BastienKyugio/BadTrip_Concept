using Unity.Netcode;
using UnityEngine;

public class InteractableItem : NetworkBehaviour
{
    public string itemName = "Item";

    [SerializeField] private GameObject visual;

    private NetworkVariable<bool> isTaken = new NetworkVariable<bool>(false);

    private void Update()
    {
        visual.SetActive(!isTaken.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InteractServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"[SERVER] InteractServerRpc appelé par client {serverRpcParams.Receive.SenderClientId}");

        if (isTaken.Value) return;

        isTaken.Value = true;

        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        var player = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
        if (player == null)
        {
            Debug.LogError($"[SERVER] PlayerObject introuvable pour client {senderClientId}");
            return;
        }

        var holder = player.GetComponent<PlayerItemHolder>();
        if (holder == null)
        {
            Debug.LogError($"[SERVER] PlayerItemHolder manquant sur PlayerObject de client {senderClientId}");
            return;
        }
        if (holder != null)
        {
            // Appelle une méthode SERVER sur ce joueur, qui va envoyer un ClientRpc depuis SON script

            holder.GiveItemToOwnerClientRpc(itemName);
            Debug.Log($"[SERVER] Appel de GiveItemToOwnerClientRpc sur le client {senderClientId}");

        }
    }

}
