using UnityEngine;

/// <summary>
/// Interface for all interactable objects in the game
/// Implement this on any object the player should be able to interact with
/// Examples: Doors, Items, NPCs, Notes
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when player presses the interact key (E)
    /// </summary>
    /// <param name="player">The player GameObject that initiated the interaction</param>
    void Interact(GameObject player);
    
    /// <summary>
    /// Returns the text to display in the UI prompt
    /// Example: "[E] Open Door" or "[E] Pick up Key"
    /// </summary>
    string GetInteractPrompt();
    
    /// <summary>
    /// Returns true if this object can currently be interacted with
    /// Use this to disable interaction based on game state
    /// </summary>
    bool CanInteract();
    
    /// <summary>
    /// Distance at which player can interact with this object
    /// Typically 2-3 meters
    /// </summary>
    float GetInteractionDistance();
}