using UnityEngine;
using System;

/// <summary>
/// Central game state manager - Phase 1 MVP version
/// Attach to a GameObject in your scene
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("Core Systems - Assign in Inspector")]
    public LoopManager loopManager;
    public GameModeManager gameModeManager;
    
    [Header("Game State")]
    public int currentLoopNumber = 1;
    public bool isGamePaused = false;
    
    // Events
    public static event Action OnGameInitialized;
    public static event Action<int> OnLoopIncremented;
    
    protected override void Awake()
    {
        base.Awake();
        InitializeSystems();
    }
    
    private void InitializeSystems()
    {
        Debug.Log("[GameManager] Initializing game systems...");
        
        // Initialize managers in order
        if (loopManager != null)
            loopManager.Initialize();
        else
            Debug.LogError("[GameManager] LoopManager reference missing! Assign in Inspector.");
        
        if (gameModeManager != null)
            gameModeManager.Initialize();
        else
            Debug.LogError("[GameManager] GameModeManager reference missing! Assign in Inspector.");
        
        OnGameInitialized?.Invoke();
        Debug.Log($"[GameManager] Game initialized. Starting Loop {currentLoopNumber}");
    }
    
    public void IncrementLoop()
    {
        currentLoopNumber++;
        OnLoopIncremented?.Invoke(currentLoopNumber);
        Debug.Log($"[GameManager] Loop incremented to {currentLoopNumber}");
    }
    
    public void PauseGame(bool pause)
    {
        isGamePaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }
    
    // Debug functions (remove in final build)
    private void Update()
    {
        // Press R to manually trigger reset (for testing)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("[GameManager] Manual reset triggered (R key)");
            if (loopManager != null)
                loopManager.TriggerLoopReset();
        }
    }
}