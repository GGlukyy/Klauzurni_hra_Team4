using UnityEngine;
using UnityEngine.Events; // Důležité pro události (cutscény) v Editoru

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Nastavení Předmětu")]
    [Tooltip("Název předmětu pro odemykání (např. 'Klic_WC', 'Karta_Level1', 'Sprej')")]
    public string itemName; 

    [Header("Příběhové události (Volitelné)")]
    [Tooltip("Spustí se pouze při ÚPLNĚ PRVNÍM sebrání předmětu (např. cutscéna).")]
    public UnityEvent onFirstPickup;

    // HideInInspector skryje tyto proměnné v Unity, protože si je skript najde sám v Awake
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider coll;

    // Hlídač, který zajistí, že se cutscéna nespustí dvakrát
    private bool hasBeenPickedUpBefore = false; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    // Tuto funkci automaticky zavolá tvůj paprsek (PlayerInteract), když klikneš levým
    public void Interact()
    {
        if (PlayerInventory.Instance != null)
        {
            // 1. Obyčejné zvednutí do inventáře (tvoje mechanika zůstává)
            PlayerInventory.Instance.TryPickup(this);

            // 2. Pokud to zvedáš poprvé, spusť události
            if (!hasBeenPickedUpBefore)
            {
                hasBeenPickedUpBefore = true;
                
                // Invoke() odpálí vše, co si v Editoru do tohoto políčka naklikáš
                onFirstPickup?.Invoke(); 
            }
        }
        else
        {
            Debug.LogError("PickupItem: Ve scéně není PlayerInventory!");
        }
    }
}