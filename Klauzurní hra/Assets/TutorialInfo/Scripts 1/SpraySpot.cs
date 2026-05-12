using UnityEngine;
using System.Collections;

public class SpraySpot : MonoBehaviour, IInteractable
{
    [Header("Spray Settings")]
    [Tooltip("Add multiple prefabs to randomize tags")]
    public GameObject[] decalPrefabs; 
    public float fadeSpeed = 2f;
    
    [Tooltip("How many times can this spot be sprayed?")]
    public int maxSprays = 1;

    [Header("Audio")]
    public AudioSource sprayAudio; 

    private int currentSprays = 0;

    public void Interact()
    {
        // Kontrola: Má hráč něco v ruce?
        PickupItem currentItem = PlayerInventory.Instance.GetCurrentItem();

        // Kontrola: Je to, co drží, opravdu sprej? (Pomocí Tagu)
        if (currentItem == null || !currentItem.CompareTag("SprayCan"))
        {
            Debug.Log("Musíš držet sprej v ruce!");
            return; 
        }

        if (currentSprays >= maxSprays) return; 
        if (decalPrefabs.Length == 0) return; 

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
            int randomIndex = Random.Range(0, decalPrefabs.Length);
            GameObject selectedDecal = decalPrefabs[randomIndex];

            GameObject decal = Instantiate(selectedDecal, hit.point, Quaternion.LookRotation(hit.normal));
            decal.transform.position += hit.normal * 0.005f; // Z-fighting fix

            if (sprayAudio != null) sprayAudio.Play();
            StartCoroutine(FadeInDecal(decal));

            if (SanityManager.Instance != null)
            {
                SanityManager.Instance.AddSpray();
            }

            currentSprays++; 
            
            // Lock interaction only when max sprays are reached
            if (currentSprays >= maxSprays)
            {
                gameObject.layer = LayerMask.NameToLayer("Default"); 
            }
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