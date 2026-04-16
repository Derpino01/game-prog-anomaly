using UnityEngine;

/// <summary>
/// Interactable item that can be picked up and added to inventory
/// Attach to any item GameObject you want the player to collect
/// IMPORTANT: Set GameObject layer to "Interactable"
/// </summary>
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Item Configuration - REQUIRED")]
    [Tooltip("Drag the InventoryItem ScriptableObject here")]
    [SerializeField] private InventoryItem item;
    
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 2f;
    
    [Header("Optional: Pickup Effects")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupVFX;
    
    public void Interact(GameObject player)
    {
        if (item == null)
        {
            Debug.LogError("[PickupItem] No InventoryItem assigned! Cannot pick up.");
            return;
        }
        
        // Get player's inventory
        InventoryManager inventory = player.GetComponent<InventoryManager>();
        
        if (inventory == null)
        {
            Debug.LogError("[PickupItem] Player has no InventoryManager component!");
            return;
        }
        
        // Try to add item
        if (inventory.AddItem(item))
        {
            Debug.Log($"[PickupItem] Player picked up: {item.itemName}");
            
            // Play pickup sound (if assigned)
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            
            // Spawn VFX (if assigned)
            if (pickupVFX != null)
            {
                Instantiate(pickupVFX, transform.position, Quaternion.identity);
            }
            
            // Destroy this pickup object
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[PickupItem] Inventory full - could not pick up item");
        }
    }
    
    public string GetInteractPrompt()
    {
        if (item != null)
        {
            return $"[E] Pick up {item.itemName}";
        }
        return "[E] Pick up";
    }
    
    public bool CanInteract()
    {
        return item != null; // Can only interact if item is assigned
    }
    
    public float GetInteractionDistance()
    {
        return interactionDistance;
    }
    
    // Validate in editor
    private void OnValidate()
    {
        if (item == null)
        {
            Debug.LogWarning($"[PickupItem] {gameObject.name} has no InventoryItem assigned!");
        }
        
        // Check layer
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            Debug.LogWarning($"[PickupItem] {gameObject.name} should be on 'Interactable' layer!");
        }
    }
}