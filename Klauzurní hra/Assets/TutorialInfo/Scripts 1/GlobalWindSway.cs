using UnityEngine;

[ExecuteInEditMode]
public class GlobalWindSway : MonoBehaviour
{
    [Header("Global Wind Settings")]
    public float windSpeed = 2.0f;        // Jak rychle se vlna vìtru pohybuje scénou
    public float windScale = 0.5f;        // Jak velké jsou vlny vìtru (menší èíslo = vìtší, plynulejší vlny)
    public float windStrength = 0.2f;     // Maximální náklon trávy

    [Header("Individual Variance")]
    [Range(0f, 1f)]
    public float individuality = 0.15f;   // Jak moc se tento konkrétní trs liší od ostatních (0 = jedou pøesnì jako jeden muž)

    private MeshFilter meshFilter;
    private float objectUniqueOffset;

    void Start()
    {
        // Malý pevný offset pro každý trs, aby se pøi zapnuté individualitì jemnì lišily
        objectUniqueOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 1. Získáme pozici objektu ve svìtì
        Vector3 worldPos = transform.position;

        // 2. Vypoèítáme pozici ve vìtrné mapì (posun v èase simuluje bìžící vítr)
        float windX = (worldPos.x * windScale) + (Time.time * windSpeed);
        float windZ = (worldPos.z * windScale) + (Time.time * windSpeed * 0.2f); // mírný diagonalní smìr

        // 3. Vzorkujeme Perlinùv šum (vrací hodnotu 0 až 1) a upravíme ji na rozsah -0.5 až 0.5
        float globalWind = Mathf.PerlinNoise(windX, windZ) - 0.5f;

        // 4. Pøidáme drobnou individuální turbulenci pro tento konkrétní kus trávy
        float localNoise = Mathf.Sin(Time.time * windSpeed * 2f + objectUniqueOffset) * individuality * 0.2f;

        // Výsledný celkový náklon
        float totalSway = (globalWind + localNoise) * windStrength;

        // 5. Matice pro skosení (Shear) - drží spodek na zemi, vršek ohýbá
        Matrix4x4 skewMatrix = Matrix4x4.identity;
        skewMatrix[0, 1] = totalSway; // Ohýbání na ose X. Pokud chceš osu Z, zmìò na skewMatrix[2, 1]

        // Vykreslení modifikovaného meshe
        Graphics.DrawMesh(
            GetComponent<MeshFilter>().sharedMesh,
            transform.localToWorldMatrix * skewMatrix,
            GetComponent<MeshRenderer>().sharedMaterial,
            gameObject.layer
        );
    }

    void OnEnable()
    {
        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().enabled = false;
    }

    void OnDisable()
    {
        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().enabled = true;
    }
}