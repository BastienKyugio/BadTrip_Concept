using Unity.Netcode;
using UnityEngine;

public class NetworkedVehicleController : NetworkBehaviour
{
    public Rigidbody rb;

    private const ulong NO_DRIVER = ulong.MaxValue;

    public float maxSpeed = 20f;       
    public float maxTurnTorque = 15000f;

    private NetworkVariable<ulong> driverClientId = new NetworkVariable<ulong>(NO_DRIVER);

    public ulong DriverClientId => driverClientId.Value;

    public float accelerationForce = 5000f;
    public float turnTorque = 3000f;

    public override void OnNetworkSpawn()
    {
        driverClientId.OnValueChanged += OnDriverChanged;
    }

    public override void OnNetworkDespawn()
    {
        driverClientId.OnValueChanged -= OnDriverChanged;
    }

    private void OnDriverChanged(ulong previousDriver, ulong newDriver)
    {
        Debug.Log($"Driver changed from {previousDriver} to {newDriver}");
        NotifyClientsDriverChangedClientRpc(newDriver);
    }

    [ClientRpc]
    private void NotifyClientsDriverChangedClientRpc(ulong newDriver)
    {
        var player = NetworkManager.Singleton.LocalClient.PlayerObject;
        if (player == null) return;

        var interaction = player.GetComponent<PlayerVehicleInteractionNetwork>();
        if (interaction == null) return;

        if (newDriver != NO_DRIVER && NetworkManager.Singleton.LocalClientId == newDriver)
        {
            Debug.Log("I am now driving");
            interaction.SetDrivingState(true);
        }
        else
        {
            Debug.Log("I am not driving");
            interaction.SetDrivingState(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestTakeControlServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;
        Debug.Log($"RequestTakeControlServerRpc received from client {requesterId}, current driver: {driverClientId.Value}");

        if (driverClientId.Value == NO_DRIVER)
        {
            driverClientId.Value = requesterId;
            Debug.Log($"DriverClientId assigned to {requesterId}");
        }
        else
        {
            Debug.Log("Vehicle already driven by someone");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReleaseControlServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong requesterId = rpcParams.Receive.SenderClientId;
        Debug.Log($"ReleaseControlServerRpc received from client {requesterId}, current driver: {driverClientId.Value}");

        if (driverClientId.Value == requesterId)
        {
            driverClientId.Value = NO_DRIVER;
            Debug.Log("DriverClientId reset to NO_DRIVER (no driver)");
        }
    }

    void FixedUpdate()
    {
        if (!IsSpawned) return;

        if (driverClientId.Value != NetworkManager.Singleton.LocalClientId) return;

        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // Appliquer force d'accélération
        rb.AddForce(transform.forward * moveInput * accelerationForce * Time.fixedDeltaTime);

        // Appliquer couple limité
        float appliedTurnTorque = Mathf.Clamp(turnInput * turnTorque, -maxTurnTorque, maxTurnTorque);
        rb.AddTorque(Vector3.up * appliedTurnTorque * Time.fixedDeltaTime);

        // Limiter la vitesse linéaire (vectoriel)
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // Limiter la vitesse angulaire autour de Y
        Vector3 angVel = rb.angularVelocity;
        float maxAngularSpeed = maxTurnTorque / rb.mass; // approximation
        if (Mathf.Abs(angVel.y) > maxAngularSpeed)
        {
            angVel.y = Mathf.Sign(angVel.y) * maxAngularSpeed;
            rb.angularVelocity = angVel;
        }
    }

}
