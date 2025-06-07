using Unity.Netcode;
using UnityEngine;

public class PlayerVehicleInteractionNetwork : NetworkBehaviour
{
    public Transform playerCamera;
    public NetworkedVehicleController vehicle;
    public Transform driverSeat;

    private bool isDriving = false;
    private CharacterController characterController;

    private const ulong NO_DRIVER = ulong.MaxValue;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        if (vehicle == null)
        {
            vehicle = GameObject.FindWithTag("Van")?.GetComponent<NetworkedVehicleController>();
        }
        if (driverSeat == null && vehicle != null)
        {
            driverSeat = vehicle.transform.Find("DriverSeat");
        }

        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        if (!IsOwner) return; // Only local player

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isDriving && Vector3.Distance(transform.position, vehicle.transform.position) < 5f)
            {
                vehicle.RequestTakeControlServerRpc();
                Debug.Log("RequestTakeControlServerRpc called");
            }
            else if (isDriving)
            {
                vehicle.ReleaseControlServerRpc();
                Debug.Log("ReleaseControlServerRpc called");
            }
        }

        if (isDriving)
        {
            if (characterController.enabled)
                characterController.enabled = false;

            // Position player in driver seat
            transform.position = driverSeat.position;
            transform.rotation = driverSeat.rotation;

            // Camera at driver seat position
            playerCamera.position = driverSeat.position;
            playerCamera.rotation = driverSeat.rotation;
        }
        else
        {
            if (!characterController.enabled)
                characterController.enabled = true;

            // Reset camera to default FPS position (adapt if needed)
            playerCamera.localPosition = new Vector3(0, 1.6f, 0);
            playerCamera.localRotation = Quaternion.identity;
        }
    }

    public void SetDrivingState(bool driving)
    {
        Debug.Log($"SetDrivingState called with {driving} on client {NetworkManager.Singleton.LocalClientId}");
        isDriving = driving;
    }
}
