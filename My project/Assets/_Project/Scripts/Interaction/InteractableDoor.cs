using UnityEngine;

/// <summary>
/// Interactable door that can be opened/closed, with optional lock requirement
/// Attach to door GameObject
/// IMPORTANT: Set GameObject layer to "Interactable"
/// </summary>
public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [Tooltip("Item ID required to unlock (e.g., 'key_chemistry')")]
    [SerializeField] private string requiredKeyID = "";
    [SerializeField] private float interactionDistance = 2.5f;
    
    [Header("Animation (Optional)")]
    [Tooltip("If you have an Animator on the door")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";
    
    [Header("Simple Rotation (If no animator)")]
    [SerializeField] private bool useSimpleRotation = true;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;
    [SerializeField] private AudioClip doorLockedSound;
    [SerializeField] private AudioClip doorUnlockSound;
    
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isRotating = false;
    
    private void Start()
    {
        if (useSimpleRotation)
        {
            closedRotation = transform.rotation;
            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        }
    }
    
    private void Update()
    {
        if (useSimpleRotation && isRotating)
        {
            // Smoothly rotate door
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            
            // Stop when close enough
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isRotating = false;
            }
        }
    }
    
    public void Interact(GameObject player)
    {
        if (isLocked)
        {
            // Check if player has required key
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            
            if (inventory != null && !string.IsNullOrEmpty(requiredKeyID))
            {
                if (inventory.HasItem(requiredKeyID))
                {
                    // Unlock the door
                    UnlockDoor();
                }
                else
                {
                    // Door is locked, no key
                    Debug.Log($"[Door] Locked. Requires key: {requiredKeyID}");
                    PlaySound(doorLockedSound);
                    return;
                }
            }
            else
            {
                // No inventory or key requirement
                Debug.Log("[Door] Locked.");
                PlaySound(doorLockedSound);
                return;
            }
        }
        
        // Toggle door state
        ToggleDoor();
    }
    
    private void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("[Door] Unlocked!");
        PlaySound(doorUnlockSound);
        
        // Optionally, remove key from inventory after use
        // inventory.RemoveItem(inventory.GetItem(requiredKeyID));
    }
    
    private void ToggleDoor()
    {
        isOpen = !isOpen;
        Debug.Log($"[Door] {(isOpen ? "Opening" : "Closing")} door");
        
        // Play sound
        PlaySound(isOpen ? doorOpenSound : doorCloseSound);
        
        // Use animator or simple rotation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger(isOpen ? openTrigger : closeTrigger);
        }
        else if (useSimpleRotation)
        {
            isRotating = true;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
    
    public string GetInteractPrompt()
    {
        if (isLocked)
        {
            return "[E] Locked";
        }
        return isOpen ? "[E] Close Door" : "[E] Open Door";
    }
    
    public bool CanInteract()
    {
        return true;
    }
    
    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
    
    // Public methods for scripted events
    public void SetLocked(bool locked)
    {
        isLocked = locked;
    }
    
    public void SetOpen(bool open)
    {
        if (open != isOpen)
        {
            ToggleDoor();
        }
    }
    
    // Validate in editor
    private void OnValidate()
    {
        // Check layer
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            Debug.LogWarning($"[InteractableDoor] {gameObject.name} should be on 'Interactable' layer!");
        }
    }
}