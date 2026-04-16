using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    [Header("Nastavení Předmětu")]
    [Tooltip("Název předmětu pro odemykání (např. 'Klic_WC', 'Karta_Level1', 'Sprej')")]
    public string itemName; 

    public Rigidbody rb;
    public Collider coll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
    }

    // Tuto funkci automaticky zavolá tvůj paprsek (PlayerInteract), když klikneš levým
    public void Interact()
    {
        // Řekneme inventáři hráče, aby se pokusil tento předmět sebrat
        PlayerInventory.Instance.TryPickup(this);
    }
}