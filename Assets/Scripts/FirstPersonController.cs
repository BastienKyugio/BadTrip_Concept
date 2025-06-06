using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;


public class FirstPersonController : NetworkBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Jumping / Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalVelocity;
    private float verticalRotation;

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTriggered ? sprintMultiplier: 1);


    void Start()
    {
        Debug.Log($"{gameObject.name} — IsOwner: {IsOwner} — IsLocalPlayer: {IsLocalPlayer}");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (IsOwner)
        {
            mainCamera.enabled = true;
            characterController.enabled = true;

            if (mainCamera.GetComponent<AudioListener>() == null)
            {
                mainCamera.gameObject.AddComponent<AudioListener>();
            }
        }
        else
        {
            characterController.enabled = false;
            mainCamera.enabled = false;

            AudioListener al = mainCamera.GetComponent<AudioListener>();
            if (al != null)
                al.enabled = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirecion()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }

    private void HandleMovement()
    {
        if (!IsOwner) return;

        Vector3 worldDirection = CalculateWorldDirecion();

        // Mouvement horizontal
        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        // Saut & gravité
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; // petit push pour rester au sol

            if (playerInputHandler.JumpTriggered)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        currentMovement.y = verticalVelocity;

        characterController.Move(currentMovement * Time.deltaTime);
    }


    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }
}
