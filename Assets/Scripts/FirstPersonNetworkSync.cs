using Unity.Netcode;
using UnityEngine;

public class FirstPersonNetworkSync : NetworkBehaviour
{
    [SerializeField] private float positionUpdateThreshold = 0.05f;
    [SerializeField] private float rotationUpdateThreshold = 1f; // degrés
    [SerializeField] private float interpolationSpeed = 10f;

    private NetworkVariable<Vector3> networkedPosition = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<Quaternion> networkedRotation = new NetworkVariable<Quaternion>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    private Vector3 lastSentPosition;
    private Quaternion lastSentRotation;

    void Update()
    {
        if (IsOwner)
        {
            // Position
            if (Vector3.Distance(transform.position, lastSentPosition) > positionUpdateThreshold)
            {
                networkedPosition.Value = transform.position;
                lastSentPosition = transform.position;
            }

            // Rotation Y uniquement (ignore X et Z pour FPS)
            float yRotationDelta = Mathf.Abs(transform.rotation.eulerAngles.y - lastSentRotation.eulerAngles.y);
            if (yRotationDelta > rotationUpdateThreshold)
            {
                Quaternion rotationYOnly = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
                networkedRotation.Value = rotationYOnly;
                lastSentRotation = rotationYOnly;
            }
        }
        else
        {
            // Interpolation de position
            transform.position = Vector3.Lerp(
                transform.position,
                networkedPosition.Value,
                Time.deltaTime * interpolationSpeed
            );

            // Interpolation de rotation Y uniquement
            Quaternion targetRotation = Quaternion.Euler(0f, networkedRotation.Value.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * interpolationSpeed
            );
        }
    }
}
