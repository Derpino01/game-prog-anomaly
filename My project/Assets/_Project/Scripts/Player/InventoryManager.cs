using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages player inventory with loop persistence support
/// Phase 1 MVP - Basic add/remove/check functionality
/// Attach to Player GameObject
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int maxInventorySize = 20;
    
    [Header("Debug - Current Items (View Only)")]
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();
    
    // Events
    public event System.Action<InventoryItem> OnItemAdded;
    public event System.Action<InventoryItem> OnItemRemoved;
    
    public bool AddItem(InventoryItem item)
    {
        if (inventory.Count >= maxInventorySize)
        {
            Debug.LogWarning("[Inventory] Inventory full! Cannot add more items.");
            return false;
        }
        
        inventory.Add(item);
        OnItemAdded?.Invoke(item);
        Debug.Log($"[Inventory] Added: {item.itemName}");
        return true;
    }
    
    public bool RemoveItem(InventoryItem item)
    {
        if (inventory.Remove(item))
        {
            OnItemRemoved?.Invoke(item);
            Debug.Log($"[Inventory] Removed: {item.itemName}");
            return true;
        }
        return false;
    }
    
    public bool HasItem(string itemID)
    {
        return inventory.Any(item => item.itemID == itemID);
    }
    
    public InventoryItem GetItem(string itemID)
    {
        return inventory.FirstOrDefault(item => item.itemID == itemID);
    }
    
    /// <summary>
    /// Returns list of item IDs that are marked as persistent
    /// Used by LoopManager to save across resets
    /// </summary>
    public List<string> GetPersistentItems()
    {
        List<string> persistentIDs = inventory
            .Where(item => item.isPersistent)
            .Select(item => item.itemID)
            .ToList();
        
        return persistentIDs;
    }
    
    /// <summary>
    /// Restores persistent items after loop reset
    /// IMPORTANT: Items must be in Assets/_Project/Resources/Items/ folder
    /// </summary>
    public void RestorePersistentItems(List<string> itemIDs)
    {
        inventory.Clear();
        
        foreach (string id in itemIDs)
        {
            // Load item from Resources folder
            InventoryItem item = Resources.Load<InventoryItem>($"Items/{id}");
            if (item != null)
            {
                inventory.Add(item);
                Debug.Log($"[Inventory] Restored: {item.itemName}");
            }
            else
            {
                Debug.LogError($"[Inventory] Could not find item: {id}. Make sure it's in Resources/Items/ folder!");
            }
        }
        
        // Notify UI to refresh
        OnItemAdded?.Invoke(null);
    }
    
    public List<InventoryItem> GetAllItems()
    {
        return new List<InventoryItem>(inventory);
    }
    
    public int GetItemCount()
    {
        return inventory.Count;
    }
    
    // Debug - print inventory to console
    [ContextMenu("Print Inventory")]
    public void PrintInventory()
    {
        Debug.Log($"=== INVENTORY ({inventory.Count}/{maxInventorySize}) ===");
        foreach (var item in inventory)
        {
            string persistent = item.isPersistent ? "[PERSISTENT]" : "";
            Debug.Log($"- {item.itemName} ({item.itemID}) {persistent}");
        }
    }
}