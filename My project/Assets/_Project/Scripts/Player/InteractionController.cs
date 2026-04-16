using UnityEngine;
using TMPro;

/// <summary>
/// Player-side interaction detection and handling
/// Uses raycast to detect IInteractable objects
/// Attach to Player GameObject
/// Requires: Camera reference and UI text for prompts
/// </summary>
public class InteractionController : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxRaycastDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    
    [Header("UI - Assign TextMeshPro Text")]
    [SerializeField] private TextMeshProUGUI interactPromptText;
    
    private IInteractable currentInteractable;
    private GameObject currentInteractableObject;
    
    private void Start()
    {
        // If camera not assigned, find it
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("[InteractionController] No camera found! Assign camera in inspector.");
            }
        }
        
        // Hide prompt by default
        if (interactPromptText != null)
        {
            interactPromptText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[InteractionController] Interact Prompt Text not assigned! Won't show prompts.");
        }
    }
    
    private void Update()
    {
        DetectInteractable();
        HandleInteractionInput();
    }
    
    private void DetectInteractable()
    {
        if (playerCamera == null) return;
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        // Cast ray to detect interactables
        if (Physics.Raycast(ray, out hit, maxRaycastDistance, interactableLayer))
        {
            // Check if hit object has IInteractable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != null && interactable.CanInteract())
            {
                // Check distance
                float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                
                if (distance <= interactable.GetInteractionDistance())
                {
                    // Valid interactable found
                    currentInteractable = interactable;
                    currentInteractableObject = hit.collider.gameObject;
                    ShowPrompt(interactable.GetInteractPrompt());
                    return;
                }
            }
        }
        
        // No valid interactable found
        ClearInteractable();
    }
    
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Debug.Log($"[InteractionController] Interacting with: {currentInteractableObject.name}");
            currentInteractable.Interact(gameObject);
        }
    }
    
    private void ShowPrompt(string text)
    {
        if (interactPromptText != null)
        {
            interactPromptText.text = text;
            interactPromptText.gameObject.SetActive(true);
        }
    }
    
    private void ClearInteractable()
    {
        currentInteractable = null;
        currentInteractableObject = null;
        
        if (interactPromptText != null)
        {
            interactPromptText.gameObject.SetActive(false);
        }
    }
    
    // Debug - draw raycast in scene view
    private void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxRaycastDistance);
        }
    }
}