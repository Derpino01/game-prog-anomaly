using UnityEngine;
using System;

public enum GameMode
{
    StableReality,
    FracturedReality
}

/// <summary>
/// Controls genre transitions between Stable and Fractured reality
/// Phase 1 MVP - Basic implementation
/// Attach to a GameObject in your scene
/// </summary>
public class GameModeManager : MonoBehaviour
{
    [Header("Mode Configuration")]
    [SerializeField] private GameMode currentMode = GameMode.StableReality;
    [SerializeField] private float timeUntilBell = 60f; // 1 minute in Stable before bell
    [SerializeField] private float warningTime = 10f; // Warning 10s before bell
    
    private float modeTimer = 0f;
    private bool bellWarningTriggered = false;
    
    // Events
    public static event Action OnBellRing;
    public static event Action<GameMode> OnModeChanged;
    public static event Action OnBellWarning;
    
    public GameMode CurrentMode => currentMode;
    
    public void Initialize()
    {
        Debug.Log("[GameModeManager] Initializing game mode system");
        SetMode(GameMode.StableReality);
        modeTimer = 0f;
        bellWarningTriggered = false;
    }
    
    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGamePaused) 
            return;
        
        // Only tick timer in Stable Reality
        if (currentMode == GameMode.StableReality)
        {
            modeTimer += Time.deltaTime;
            
            // Warning before bell
            if (!bellWarningTriggered && modeTimer >= (timeUntilBell - warningTime))
            {
                bellWarningTriggered = true;
                OnBellWarning?.Invoke();
                Debug.Log("[GameModeManager] WARNING: Bell ringing in 10 seconds!");
            }
            
            // Ring the bell
            if (modeTimer >= timeUntilBell)
            {
                RingBell();
            }
        }
        
        // Debug: Press T to manually toggle mode
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[GameModeManager] Manual mode toggle (T key)");
            ToggleMode();
        }
    }
    
    public void RingBell()
    {
        Debug.Log("[GameModeManager] BELL RING - Transitioning to Fractured Reality");
        OnBellRing?.Invoke();
        SetMode(GameMode.FracturedReality);
        modeTimer = 0f;
        bellWarningTriggered = false;
    }
    
    public void SetMode(GameMode newMode)
    {
        if (currentMode == newMode) return;
        
        GameMode previousMode = currentMode;
        currentMode = newMode;
        
        OnModeChanged?.Invoke(currentMode);
        
        Debug.Log($"[GameModeManager] Mode changed: {previousMode} → {currentMode}");
    }
    
    // For testing - manual mode toggle
    public void ToggleMode()
    {
        if (currentMode == GameMode.StableReality)
            SetMode(GameMode.FracturedReality);
        else
            SetMode(GameMode.StableReality);
    }
}