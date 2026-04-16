using UnityEngine;

/// <summary>
/// Manages player's sanity/lucidity meter
/// Phase 1 MVP - Basic implementation with drain and recovery
/// Attach to Player GameObject
/// </summary>
public class LucidityManager : MonoBehaviour
{
    [Header("Lucidity Settings")]
    [SerializeField] private float maxLucidity = 100f;
    [SerializeField] private float currentLucidity = 100f;
    
    [Header("Drain Settings")]
    [SerializeField] private float passiveDrainRate = 0.5f; // Per second in Stable
    [SerializeField] private float fracturedDrainMultiplier = 2f; // 2x faster in Fractured
    [SerializeField] private float anomalyDrainAmount = 5f; // Instant drain when seeing anomaly
    
    [Header("Recovery")]
    [SerializeField] private float recoveryRate = 1f; // When in safe zones
    
    [Header("Visual Feedback")]
    [SerializeField] private float lowLucidityThreshold = 30f; // When to show low sanity effects
    
    private bool isInFracturedReality = false;
    private bool isInSafeZone = false;
    
    // Events
    public event System.Action<float> OnLucidityChanged;
    public event System.Action OnLucidityDepleted;
    public event System.Action<float> OnLucidityPercentChanged;
    
    // Properties
    public float CurrentLucidity => currentLucidity;
    public float MaxLucidity => maxLucidity;
    public float LucidityPercent => currentLucidity / maxLucidity;
    public bool IsLowLucidity => currentLucidity <= lowLucidityThreshold;
    
    private void Start()
    {
        // Subscribe to game mode changes
        GameModeManager.OnModeChanged += HandleModeChange;
        
        // Reset lucidity on loop
        LoopManager.OnLoopReset += ResetLucidity;
        
        currentLucidity = maxLucidity;
    }
    
    private void Update()
    {
        UpdateLucidityDrain();
    }
    
    private void UpdateLucidityDrain()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused)
            return;
        
        float drainRate = passiveDrainRate;
        
        // Faster drain in Fractured Reality
        if (isInFracturedReality)
        {
            drainRate *= fracturedDrainMultiplier;
        }
        
        // Recovery in safe zones
        if (isInSafeZone)
        {
            ModifyLucidity(recoveryRate * Time.deltaTime);
        }
        else
        {
            // Passive drain
            ModifyLucidity(-drainRate * Time.deltaTime);
        }
    }
    
    public void ModifyLucidity(float amount)
    {
        float previousLucidity = currentLucidity;
        currentLucidity = Mathf.Clamp(currentLucidity + amount, 0f, maxLucidity);
        
        if (currentLucidity != previousLucidity)
        {
            OnLucidityChanged?.Invoke(currentLucidity);
            OnLucidityPercentChanged?.Invoke(LucidityPercent);
            
            // Log for debugging
            if (amount < 0)
            {
                Debug.Log($"[Lucidity] Drained: {Mathf.Abs(amount):F1} | Current: {currentLucidity:F1}/{maxLucidity}");
            }
        }
        
        // Check for depletion
        if (currentLucidity <= 0f)
        {
            OnLucidityDepleted?.Invoke();
            HandleLucidityDepleted();
        }
    }
    
    /// <summary>
    /// Call this when player witnesses an anomaly
    /// </summary>
    public void OnAnomalyWitnessed()
    {
        Debug.Log($"[Lucidity] Anomaly witnessed! Draining {anomalyDrainAmount} lucidity");
        ModifyLucidity(-anomalyDrainAmount);
    }
    
    private void HandleModeChange(GameMode newMode)
    {
        isInFracturedReality = (newMode == GameMode.FracturedReality);
        Debug.Log($"[Lucidity] Mode changed to {newMode}. Fractured drain: {isInFracturedReality}");
    }
    
    private void HandleLucidityDepleted()
    {
        Debug.LogWarning("[Lucidity] Lucidity depleted! Triggering death/reset...");
        
        // Trigger loop reset or death
        LoopManager loopManager = FindObjectOfType<LoopManager>();
        if (loopManager != null)
        {
            loopManager.TriggerLoopReset();
        }
    }
    
    public void ResetLucidity()
    {
        currentLucidity = maxLucidity;
        OnLucidityChanged?.Invoke(currentLucidity);
        OnLucidityPercentChanged?.Invoke(LucidityPercent);
        Debug.Log("[Lucidity] Reset to maximum");
    }
    
    /// <summary>
    /// Call when entering/exiting safe zones (e.g., classroom, bathroom)
    /// </summary>
    public void SetSafeZone(bool inSafeZone)
    {
        isInSafeZone = inSafeZone;
        Debug.Log($"[Lucidity] Safe zone: {isInSafeZone}");
    }
    
    // Debug - manually modify lucidity
    private void OnValidate()
    {
        // Clamp in editor
        currentLucidity = Mathf.Clamp(currentLucidity, 0f, maxLucidity);
    }
    
    private void OnDestroy()
    {
        GameModeManager.OnModeChanged -= HandleModeChange;
        LoopManager.OnLoopReset -= ResetLucidity;
    }
}