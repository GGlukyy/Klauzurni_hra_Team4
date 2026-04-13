using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PickupItem : MonoBehaviour, IInteractable
{
    public Rigidbody rb;
    public Collider coll;
    
    public string itemName; // Sem v Inspectoru napíšeš např. "Klic_Zachody"

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