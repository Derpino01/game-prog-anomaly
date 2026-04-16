using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages time-loop resets and persistence - Phase 1 MVP
/// Attach to a GameObject in your scene
/// </summary>
public class LoopManager : MonoBehaviour
{
    [Header("Loop Configuration")]
    [SerializeField] private float loopDuration = 120f; // 2 minutes for testing
    [SerializeField] private string currentSceneName;
    
    [Header("Player Spawn")]
    [SerializeField] private Transform playerSpawnPoint;
    
    private float currentLoopTime = 0f;
    private bool loopActive = false;
    private List<string> persistentInventoryItems = new List<string>();
    
    // Events
    public static event System.Action OnLoopReset;
    public static event System.Action<float> OnLoopTimeUpdated;
    
    public void Initialize()
    {
        Debug.Log("[LoopManager] Initializing loop system");
        
        // Subscribe to events
        GameModeManager.OnBellRing += TriggerLoopReset;
        
        // Get current scene name
        currentSceneName = SceneManager.GetActiveScene().name;
        
        loopActive = true;
        currentLoopTime = 0f;
    }
    
    private void Update()
    {
        if (!loopActive || GameManager.Instance.isGamePaused) 
            return;
        
        currentLoopTime += Time.deltaTime;
        OnLoopTimeUpdated?.Invoke(currentLoopTime);
        
        // Auto-reset if time runs out (backup safety)
        if (currentLoopTime >= loopDuration)
        {
            Debug.Log("[LoopManager] Loop duration exceeded - triggering reset");
            TriggerLoopReset();
        }
    }
    
    public void TriggerLoopReset()
    {
        Debug.Log("[LoopManager] Loop reset triggered");
        loopActive = false;
        StartCoroutine(ResetSequence());
    }
    
    private IEnumerator ResetSequence()
    {
        // 1. Capture persistent data
        CapturePersistentData();
        
        // 2. Visual transition
        yield return StartCoroutine(PlayResetVisuals());
        
        // 3. Reload scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneName);
        yield return asyncLoad;
        
        // Wait a frame for scene to fully load
        yield return null;
        
        // 4. Restore persistent data
        RestorePersistentData();
        
        // 5. Increment loop counter
        GameManager.Instance.IncrementLoop();
        
        // 6. Notify systems
        OnLoopReset?.Invoke();
        
        // 7. Restart loop
        currentLoopTime = 0f;
        loopActive = true;
        
        Debug.Log("[LoopManager] Loop reset complete");
    }
    
    private void CapturePersistentData()
    {
        // Find player's inventory manager
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                persistentInventoryItems = inventory.GetPersistentItems();
                Debug.Log($"[LoopManager] Captured {persistentInventoryItems.Count} persistent items");
            }
        }
        else
        {
            Debug.LogWarning("[LoopManager] Player not found! Make sure Player GameObject has tag 'Player'");
        }
    }
    
    private void RestorePersistentData()
    {
        // Find player's inventory manager
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            InventoryManager inventory = player.GetComponent<InventoryManager>();
            if (inventory != null)
            {
                inventory.RestorePersistentItems(persistentInventoryItems);
                Debug.Log($"[LoopManager] Restored {persistentInventoryItems.Count} persistent items");
            }
        }
    }
    
    private IEnumerator PlayResetVisuals()
    {
        // Simple fade to black effect
        Debug.Log("[LoopManager] Playing reset visuals");
        
        // TODO: Implement proper screen fade effect in Phase 2
        // For now, just wait
        yield return new WaitForSeconds(1f);
    }
    
    private void OnDestroy()
    {
        GameModeManager.OnBellRing -= TriggerLoopReset;
    }
}