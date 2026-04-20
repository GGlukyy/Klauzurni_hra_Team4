using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    [Header("Nastavení světla")]
    [SerializeField] private Light lightSource;
    
    [Header("Nastavení blikání")]
    public float minIntensity = 0f;
    public float maxIntensity = 10f;
    public float speed = 5f;
    public bool smoothFlash = true; // Pokud je true, světlo plynule pulzuje

    void Start()
    {
        // Pokud není světlo přiřazeno v inspectoru, zkusíme ho najít
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }
    }

    void Update()
    {
        if (lightSource == null) return;

        float t;
        if (smoothFlash)
        {
            // Plynulá vlna pomocí Sinus (0 až 1)
            t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        }
        else
        {
            // Ostré blikání (zapnuto/vypnuto)
            t = Mathf.Repeat(Time.time * speed, 1f) > 0.5f ? 1f : 0f;
        }

        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}