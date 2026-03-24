using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Vyžaduje URP
using System.Collections;

public class CorruptionManager : MonoBehaviour
{
    public static CorruptionManager Instance;

    [Header("Nastavení Post-Processingu (URP)")]
    public Volume globalVolume; 

    [Header("Balancování Efektu")]
    [Tooltip("Kolik zdí musí hráč posprejovat, aby byl efekt na MAX")]
    public int maxCorruptionPoints = 5; 
    
    [Header("Nastavení Motion Blur")]
    public float startingIntensity = 0f; 
    public float maxIntensity = 1f;     

    private int currentCorruptionPoints = 0;
    
    // OPRAVA ZDE: MotionBlur musí být dohromady, bez mezery!
    private MotionBlur motionBlurEffect;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (globalVolume != null && globalVolume.profile.TryGet(out motionBlurEffect))
        {
            motionBlurEffect.intensity.value = startingIntensity;
        }
        else
        {
            Debug.LogError("CorruptionManager: Nenašel Volume nebo Motion Blur override v profilu!");
        }
    }

    public void IncreaseCorruptionEffect()
    {
        if (currentCorruptionPoints >= maxCorruptionPoints) return; 

        currentCorruptionPoints++;
        
        float corruptionPercent = (float)currentCorruptionPoints / (float)maxCorruptionPoints;
        float newIntensity = Mathf.Lerp(startingIntensity, maxIntensity, corruptionPercent);
        
        StartCoroutine(SmoothIntensityChangeRoutine(newIntensity));
        
        Debug.Log("Sprejování má následky... Corruption bod: " + currentCorruptionPoints + "/" + maxCorruptionPoints);
    }

    private IEnumerator SmoothIntensityChangeRoutine(float targetIntensity)
    {
        if (motionBlurEffect == null) yield break;

        float currentInt = motionBlurEffect.intensity.value;
        float elapsed = 0f;
        float duration = 1f; 

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            motionBlurEffect.intensity.value = Mathf.Lerp(currentInt, targetIntensity, elapsed / duration);
            yield return null;
        }
        motionBlurEffect.intensity.value = targetIntensity; 
    }
}