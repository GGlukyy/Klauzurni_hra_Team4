using UnityEngine;

public class BrokenLight : MonoBehaviour
{
    [Header("Nastavení světla")]
    [SerializeField] private Light lightSource;
    
    [Header("Nastavení praskání (Flicker)")]
    public float minIntensity = 0f;
    public float maxIntensity = 10f;
    
    [Tooltip("Jak rychle světlo praská a mění intenzitu")]
    public float flickerSpeed = 10f;
    
    [Tooltip("True = ostré záblesky (zářivka), False = plynulejší kolísání proudu")]
    public bool sharpFlicker = false; 

    // Proměnná pro posun šumu, aby všechna světla ve hře neblikala stejně!
    private float randomOffset;

    void Start()
    {
        // Pokud není světlo přiřazeno v inspectoru, zkusíme ho najít
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }

        // Vygenerujeme náhodný počátek (seed) pro každé světlo.
        // Tím zaručíme, že pokud dáš tento script na 5 světel v jedné chodbě, 
        // každé bude blikat úplně jindy a nevytvoří se "diskotéka".
        randomOffset = Random.Range(0f, 1000f);
    }

    void Update()
    {
        if (lightSource == null) return;

        // PerlinNoise generuje organický náhodný šum od 0.0 do 1.0.
        // Osa X je náš unikátní posun, osa Y je čas, který plyne rychlostí flickerSpeed.
        float noise = Mathf.PerlinNoise(randomOffset, Time.time * flickerSpeed);

        if (sharpFlicker)
        {
            // Pokud chceme ostré blikání (zkraty), upravíme šum na čistou 0 nebo 1
            // Pokud je noise větší než 0.5, svítí naplno. Pokud ne, úplně zhasne.
            noise = noise > 0.5f ? 1f : 0f;
        }
        else
        {
            // Malý trik: umocníme noise na třetí. 
            // Světlo se bude víc držet ve tmě a jen občas ostře "prskne" nahoru.
            noise = noise * noise * noise;
        }

        // Aplikujeme vypočítaný náhodný šum na intenzitu
        lightSource.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
    }
}