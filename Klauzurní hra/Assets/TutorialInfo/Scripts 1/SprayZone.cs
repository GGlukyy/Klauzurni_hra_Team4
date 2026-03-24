using UnityEngine;
using System.Collections;

public class SprayZone : MonoBehaviour
{
    [Header("Nastavení Spreje")]
    [Tooltip("Vlož sem víc prefabů pro střídání tagů")]
    public GameObject[] decalPrefabs; // Změněno na pole (Array)
    public float fadeSpeed = 2f;   
    
    [Header("Zvuk")]
    public AudioSource sprayAudio; 

    // Hlídá, jestli už tato zeď dala hráči negativní body
    private bool hasContributedToCorruption = false;

    public void Spray(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Kontrola, jestli máš přiřazené nějaké prefaby
        if (decalPrefabs.Length == 0) return;

        // 1. Vybere náhodný tag ze seznamu
        int randomIndex = Random.Range(0, decalPrefabs.Length);
        GameObject selectedDecal = decalPrefabs[randomIndex];

        // 2. Vytvoří decal
        GameObject decal = Instantiate(selectedDecal, hitPoint, Quaternion.LookRotation(hitNormal));
        decal.transform.position += hitNormal * 0.005f;

        if (sprayAudio != null) sprayAudio.Play();
        StartCoroutine(FadeInDecal(decal));

        // 3. OCHRANA PROTI SPAMU: UI a následky se spustí jen při prvním spreji
        if (!hasContributedToCorruption)
        {
            hasContributedToCorruption = true; // Uzamkneme další bodování pro tuto zeď
            
            if (CorruptionUI.Instance != null)
            {
                CorruptionUI.Instance.ShowWarning();
            }
            
            // TADY POTOM PŘIDÁŠ BOD DO TVÉHO SYSTÉMU:
            // GameManager.Instance.AddCorruptionPoint();
        }
    }

    private IEnumerator FadeInDecal(GameObject decal)
    {
        SpriteRenderer sr = decal.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f; 
            sr.color = c;

            while (c.a < 1f)
            {
                c.a += Time.deltaTime * fadeSpeed;
                sr.color = c;
                yield return null;
            }
        }
    }
}