using UnityEngine;

/// <summary>
/// First-person player movement controller
/// Phase 1 MVP - Basic WASD + Sprint + Mouse Look
/// Requires: CharacterController component
/// Attach to your Player GameObject
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Mouse Look")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float lookXLimit = 80f;
    
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private bool canMove = true;
    
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        // Lock cursor for FPS control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // If camera not assigned, find it
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
            if (playerCamera == null)
            {
                Debug.LogError("[PlayerController] No camera found! Add a Camera as child or assign in Inspector.");
            }
        }
    }
    
    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleInput();
    }
    
    private void HandleMovement()
    {
        if (!canMove) return;
        
        // Get input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        // Calculate move direction
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        // Sprint when holding Shift
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        // Apply movement
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * moveZ + right * moveX).normalized * currentSpeed;
        
        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y = movementDirectionY;
            moveDirection.y += gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -0.5f; // Keep grounded
        }
        
        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
    }
    
    private void HandleMouseLook()
    {
        if (!canMove || playerCamera == null) return;
        
        // Rotate camera up/down
        rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        
        // Rotate player left/right
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
    }
    
    private void HandleInput()
    {
        // Unlock cursor when pressing Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Re-lock cursor when clicking
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}