using UnityEngine;
using System.Collections;

/// <summary>
/// Controls lighting transitions between game modes
/// Phase 1 MVP - Basic light intensity and color changes
/// Attach to the same GameObject as EnvironmentStateManager
/// </summary>
public class LightingController : MonoBehaviour
{
    [Header("Stable Reality Lighting - REQUIRED")]
    [Tooltip("Main directional light for daytime (usually the Sun)")]
    [SerializeField] private Light stableLight;
    [SerializeField] private Color stableSkyColor = new Color(0.5f, 0.7f, 1f);
    [SerializeField] private float stableLightIntensity = 1f;
    
    [Header("Fractured Reality Lighting - REQUIRED")]
    [Tooltip("Dim/red light for nightmare mode")]
    [SerializeField] private Light fracturedLight;
    [SerializeField] private Color fracturedSkyColor = new Color(0.1f, 0.05f, 0.05f);
    [SerializeField] private float fracturedLightIntensity = 0.2f;
    
    [Header("Fractured-Only Lights (Optional)")]
    [Tooltip("Flickering emergency lights that only appear in fractured mode")]
    [SerializeField] private GameObject[] fracturedOnlyLights;
    
    private Coroutine currentTransition;
    
    public void SetModeImmediate(GameMode mode)
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }
        
        if (mode == GameMode.StableReality)
        {
            ApplyStableLighting();
        }
        else
        {
            ApplyFracturedLighting();
        }
    }
    
    public void TransitionToMode(GameMode mode, float duration)
    {
        if (currentTransition != null)
        {
            StopCoroutine(currentTransition);
        }
        
        currentTransition = StartCoroutine(TransitionCoroutine(mode, duration));
    }
    
    private IEnumerator TransitionCoroutine(GameMode mode, float duration)
    {
        Color startSkyColor = RenderSettings.ambientSkyColor;
        Color targetSkyColor = (mode == GameMode.StableReality) ? stableSkyColor : fracturedSkyColor;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Lerp sky color
            RenderSettings.ambientSkyColor = Color.Lerp(startSkyColor, targetSkyColor, t);
            
            yield return null;
        }
        
        // Apply final state
        if (mode == GameMode.StableReality)
        {
            ApplyStableLighting();
        }
        else
        {
            ApplyFracturedLighting();
        }
        
        currentTransition = null;
    }
    
    private void ApplyStableLighting()
    {
        RenderSettings.ambientSkyColor = stableSkyColor;
        
        // Enable stable light
        if (stableLight != null)
        {
            stableLight.intensity = stableLightIntensity;
            stableLight.enabled = true;
        }
        
        // Disable fractured light
        if (fracturedLight != null)
        {
            fracturedLight.enabled = false;
        }
        
        // Disable fractured-only lights
        if (fracturedOnlyLights != null)
        {
            foreach (var lightObj in fracturedOnlyLights)
            {
                if (lightObj != null)
                    lightObj.SetActive(false);
            }
        }
        
        // Disable fog
        RenderSettings.fog = false;
        
        Debug.Log("[LightingController] Applied Stable lighting");
    }
    
    private void ApplyFracturedLighting()
    {
        RenderSettings.ambientSkyColor = fracturedSkyColor;
        
        // Disable stable light
        if (stableLight != null)
        {
            stableLight.enabled = false;
        }
        
        // Enable fractured light
        if (fracturedLight != null)
        {
            fracturedLight.intensity = fracturedLightIntensity;
            fracturedLight.enabled = true;
        }
        
        // Enable fractured-only lights
        if (fracturedOnlyLights != null)
        {
            foreach (var lightObj in fracturedOnlyLights)
            {
                if (lightObj != null)
                    lightObj.SetActive(true);
            }
        }
        
        // Enable fog for atmosphere
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.05f, 0.02f, 0.02f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.02f;
        
        Debug.Log("[LightingController] Applied Fractured lighting");
    }
    
    // Validate in editor
    private void OnValidate()
    {
        if (stableLight == null)
        {
            Debug.LogWarning("[LightingController] Stable light not assigned! Assign in Inspector.");
        }
        if (fracturedLight == null)
        {
            Debug.LogWarning("[LightingController] Fractured light not assigned! Assign in Inspector.");
        }
    }
}