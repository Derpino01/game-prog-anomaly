using UnityEngine;
using System.Collections;

/// <summary>
/// Manages environment transitions between Stable and Fractured reality
/// Phase 1 MVP - Simple show/hide approach
/// Attach to a GameObject in your scene
/// </summary>
public class EnvironmentStateManager : MonoBehaviour
{
    [Header("Environment References - REQUIRED")]
    [Tooltip("Parent GameObject containing all stable reality geometry")]
    [SerializeField] private GameObject stableEnvironmentRoot;
    
    [Tooltip("Parent GameObject containing all fractured reality geometry")]
    [SerializeField] private GameObject fracturedEnvironmentRoot;
    
    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 2f;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip bellSound;
    [SerializeField] private AudioClip transitionDroneSound;
    
    private LightingController lightingController;
    
    private void Start()
    {
        lightingController = GetComponent<LightingController>();
        
        // Subscribe to game mode events
        GameModeManager.OnModeChanged += HandleModeTransition;
        GameModeManager.OnBellRing += PlayBellSound;
        
        // Initialize in stable mode
        SetEnvironmentImmediate(GameMode.StableReality);
    }
    
    private void PlayBellSound()
    {
        if (bellSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(bellSound, Camera.main.transform.position, 1f);
        }
    }
    
    private void HandleModeTransition(GameMode newMode)
    {
        StartCoroutine(TransitionToMode(newMode));
    }
    
    private IEnumerator TransitionToMode(GameMode targetMode)
    {
        Debug.Log($"[EnvironmentStateManager] Transitioning to {targetMode}");
        
        // Play transition audio
        if (transitionDroneSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(transitionDroneSound, Camera.main.transform.position, 0.7f);
        }
        
        // Transition lighting
        if (lightingController != null)
        {
            lightingController.TransitionToMode(targetMode, transitionDuration);
        }
        
        // Wait for transition
        yield return new WaitForSeconds(transitionDuration);
        
        // Swap environments
        if (targetMode == GameMode.FracturedReality)
        {
            if (stableEnvironmentRoot != null)
                stableEnvironmentRoot.SetActive(false);
            
            if (fracturedEnvironmentRoot != null)
                fracturedEnvironmentRoot.SetActive(true);
        }
        else // Stable
        {
            if (fracturedEnvironmentRoot != null)
                fracturedEnvironmentRoot.SetActive(false);
            
            if (stableEnvironmentRoot != null)
                stableEnvironmentRoot.SetActive(true);
        }
        
        Debug.Log($"[EnvironmentStateManager] Transition complete");
    }
    
    private void SetEnvironmentImmediate(GameMode mode)
    {
        if (mode == GameMode.StableReality)
        {
            if (stableEnvironmentRoot != null)
                stableEnvironmentRoot.SetActive(true);
            if (fracturedEnvironmentRoot != null)
                fracturedEnvironmentRoot.SetActive(false);
        }
        else
        {
            if (stableEnvironmentRoot != null)
                stableEnvironmentRoot.SetActive(false);
            if (fracturedEnvironmentRoot != null)
                fracturedEnvironmentRoot.SetActive(true);
        }
        
        if (lightingController != null)
        {
            lightingController.SetModeImmediate(mode);
        }
    }
    
    private void OnDestroy()
    {
        GameModeManager.OnModeChanged -= HandleModeTransition;
        GameModeManager.OnBellRing -= PlayBellSound;
    }
    
    // Validate references in editor
    private void OnValidate()
    {
        if (stableEnvironmentRoot == null)
        {
            Debug.LogWarning("[EnvironmentStateManager] Stable environment root not assigned! Assign in Inspector.");
        }
        if (fracturedEnvironmentRoot == null)
        {
            Debug.LogWarning("[EnvironmentStateManager] Fractured environment root not assigned! Assign in Inspector.");
        }
    }
}