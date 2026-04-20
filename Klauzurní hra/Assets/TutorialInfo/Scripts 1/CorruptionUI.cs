using UnityEngine;
using TMPro;
using System.Collections;

public class CorruptionUI : MonoBehaviour
{
    public static CorruptionUI Instance;

    [Header("UI Nastavení")]
    public TextMeshProUGUI warningText; 
    public float displayTime = 2f; 
    public float fadeSpeed = 1f;   
    
    [Header("Road 96 Glitch Efekt")]
    public int flickerCount = 4;      // Kolikrát to problikne
    public float flickerSpeed = 0.08f; // Rychlost blikání

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        if (warningText != null)
        {
            Color c = warningText.color;
            c.a = 0f;
            warningText.color = c;
        }
    }

    public void ShowWarning()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FlickerAndFadeRoutine());
    }

    private IEnumerator FlickerAndFadeRoutine()
    {
        Color c = warningText.color;

        // 1. Rychlé problikávání (Glitch efekt)
        for (int i = 0; i < flickerCount; i++)
        {
            c.a = 1f;
            warningText.color = c;
            yield return new WaitForSeconds(flickerSpeed);
            
            c.a = 0.2f; // Zmizí jen částečně, ať je to víc creepy
            warningText.color = c;
            yield return new WaitForSeconds(flickerSpeed);
        }

        // 2. Chvíli svítí naplno
        c.a = 1f;
        warningText.color = c;
        yield return new WaitForSeconds(displayTime);

        // 3. Plynule zmizí
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            warningText.color = c;
            yield return null;
        }
    }
}