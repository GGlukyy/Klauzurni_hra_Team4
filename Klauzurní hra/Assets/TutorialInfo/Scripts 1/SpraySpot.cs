using UnityEngine;
using System.Collections;

public class SpraySpot : MonoBehaviour, IInteractable
{
    [Header("Nastavení Spreje")]
    [Tooltip("Vlož sem víc prefabů pro střídání tagů")]
    public GameObject[] decalPrefabs; 
    public float fadeSpeed = 2f;   
    
    [Header("Zvuk")]
    public AudioSource sprayAudio; 

    // Hlídá, jestli už tato zeď byla posprejována
    private bool hasBeenSprayed = false;

    public void Interact()
    {
        if (hasBeenSprayed) return; // Jde to jen jednou
        if (decalPrefabs.Length == 0) return; // Ochrana, kdybys zapomněl vložit prefaby

        // Uděláme si rychlý lokální Raycast, abychom zjistili PŘESNÝ bod dopadu a úhel zdi
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        // Střelíme paprsek (max na 5 metrů, ať neposprejuješ zeď na kilometr)
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            // 1. Vybere náhodný tag ze seznamu
            int randomIndex = Random.Range(0, decalPrefabs.Length);
            GameObject selectedDecal = decalPrefabs[randomIndex];

            // 2. Vytvoří decal přesně na bodu dopadu a natočí ho podle zdi (tvůj starý dobrý kód)
            GameObject decal = Instantiate(selectedDecal, hit.point, Quaternion.LookRotation(hit.normal));
            decal.transform.position += hit.normal * 0.005f; // Z-fighting fix

            if (sprayAudio != null) sprayAudio.Play();
            StartCoroutine(FadeInDecal(decal));

            // 3. PROPOJENÍ NA SANITY SYSTÉM (Vyléčí sanity, přidá bod ke špatnému konci)
            if (SanityManager.Instance != null)
            {
                SanityManager.Instance.AddSpray();
            }

            hasBeenSprayed = true; // Zamknout
            
            // 4. Vypneme interakci - změníme vrstvu na Default, aby už na to nefungoval tvůj Outline
            gameObject.layer = LayerMask.NameToLayer("Default"); 
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