using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Sanity (Korupce) Nastavení")]
    [Tooltip("0 = v pohodě, 100 = smrt / absolutní šílenství")]
    public float currentCorruption = 0f;
    public float maxCorruption = 100f;
    
    [Tooltip("Rychlost zhoršování na světle (za sekundu)")]
    public float passiveCorruptionRate = 1f;
    [Tooltip("Rychlost zhoršování ve tmě (za sekundu)")]
    public float darkCorruptionRate = 3f;

    [Header("Statistiky pro konce")]
    public int sprayCount = 0; // Kolikrát hráč sprejoval
    public int cigarettesInInventory = 0; // Kolik má cigaret

    [Header("Post-Processing Návaznost")]
    public Volume globalVolume;
    
    private Vignette vignette;
    private MotionBlur motionBlur;
    private ChromaticAberration chromaticAberration;

    private bool isInDarkness = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Získání efektů z URP Volume profilu
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out motionBlur);
            globalVolume.profile.TryGet(out chromaticAberration);
        }
    }

    private void Update()
    {
        // 1. Zvyšování korupce podle toho, kde hráč je
        float currentRate = isInDarkness ? darkCorruptionRate : passiveCorruptionRate;
        currentCorruption += currentRate * Time.deltaTime;
        currentCorruption = Mathf.Clamp(currentCorruption, 0, maxCorruption);

        if (currentCorruption >= maxCorruption)
        {
            DieFromInsanity();
        }

        // 2. Aktualizace vizuálu
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        // Převedeme korupci 0-100 na procenta 0.0 - 1.0
        float t = currentCorruption / maxCorruption;

        if (vignette != null)
        {
            // Základ 0.3, maximum např. 0.6
            vignette.intensity.value = Mathf.Lerp(0.3f, 0.6f, t);
        }

        if (motionBlur != null)
        {
            // Základ 0, maximum např. 1
            motionBlur.intensity.value = Mathf.Lerp(0f, 1f, t);
        }

        if (chromaticAberration != null)
        {
            // Základ 0.2, maximum např. 1
            chromaticAberration.intensity.value = Mathf.Lerp(0.2f, 1f, t);
        }
    }

    // --- Veřejné metody pro ostatní skripty ---

    public void HealSanity(float amount)
    {
        currentCorruption -= amount;
        currentCorruption = Mathf.Clamp(currentCorruption, 0, maxCorruption);
    }

    public void AddSpray()
    {
        sprayCount++;
        // Tady rovnou vyléčíme sanity za posprejování
        HealSanity(25f); 
    }

    public void SetDarkness(bool state)
    {
        isInDarkness = state;
    }

    private void DieFromInsanity()
    {
        Debug.Log("Hráč se zbláznil / pohltila ho tma. Konec hry.");
        // Zde zavoláš restart levelu nebo smrt
    }
}