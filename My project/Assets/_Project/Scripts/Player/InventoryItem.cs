using UnityEngine;

/// <summary>
/// ScriptableObject definition for inventory items
/// Create items via: Right-click in Project → Create → Inventory → Item
/// Save items in: Assets/_Project/Resources/Items/ folder
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [Header("Item Information")]
    [Tooltip("Unique ID like 'key_chemistry' or 'note_teacher'")]
    public string itemID;
    
    [Tooltip("Display name shown to player")]
    public string itemName;
    
    [TextArea(3, 5)]
    public string description;
    
    [Header("Visual")]
    [Tooltip("UI icon - optional for Phase 1")]
    public Sprite icon;
    
    [Tooltip("3D model for world placement")]
    public GameObject worldPrefab;
    
    [Header("Persistence")]
    [Tooltip("If TRUE, this item will survive loop resets")]
    public bool isPersistent = false;
}