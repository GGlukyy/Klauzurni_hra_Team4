using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Stav Dveří")]
    [Tooltip("Zaškrtni = zamčeno. Nezaškrtnuto = normálně jdou tahat.")]
    public bool isLocked = true; 

    [Header("Nastavení Zámku (pokud je zamčeno)")]
    [Tooltip("Přesný název předmětu z PickupItem (např. 'Klic_WC')")]
    public string requiredItemName; 
    
    [Tooltip("True = klíč po odemčení z ruky zmizí. False = zůstane v inventáři.")]
    public bool consumeKey = true; 

    public void Interact()
    {
        // Pokud už je odemčeno, zámek ignorujeme
        if (!isLocked) return;

        // Podíváme se hráči do ruky
        PickupItem heldItem = PlayerInventory.Instance.GetCurrentItem();

        // Má v ruce správný předmět?
        if (heldItem != null && heldItem.itemName == requiredItemName)
        {
            isLocked = false;
            Debug.Log("Dveře odemčeny! Teď jdou tahat.");

            if (consumeKey)
            {
                PlayerInventory.Instance.ConsumeCurrentItem(); // Sežere klíč
            }
        }
        else
        {
            Debug.Log($"Zamčeno! Potřebuješ předmět s názvem: {requiredItemName}");
            // ZDE MŮŽEŠ PŘIDAT ZVUK CLOUMÁNÍ ZAMČENOU KLIKOU
        }
    }
}