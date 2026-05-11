using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Door Status")]
    public bool isLocked = true;
    private bool isOpen = false;

    [Header("Key Settings")]
    public string requiredKeyName;
    public bool destroyKeyOnUse;

    [Header("Code Settings")]
    [Tooltip("Check this if the door is opened by the Keypad instead of a key.")]
    public bool requiresCode = false;

    public void Interact()
    {
        // 1. Otevření/zavření odemčených dveří
        if (!isLocked)
        {
            ToggleDoor();
            return;
        }

        // 2. Dveře na kód ignorují klíče v ruce
        if (requiresCode)
        {
            Debug.Log("This door is locked electronically. You need a code.");
            return; 
        }

        // 3. LOGIKA PRO KLÍČ V AKTIVNÍ RUCE
        bool hasKey = false; 

        if (PlayerInventory.Instance != null)
        {
            // Načtení předmětu, který hráč právě drží
            PickupItem itemInHand = PlayerInventory.Instance.GetCurrentItem();
            
            // Kontrola, zda něco drží a jestli se itemName přesně shoduje
            if (itemInHand != null && itemInHand.itemName == requiredKeyName)
            {
                hasKey = true;
            }
        }

        if (hasKey)
        {
            isLocked = false;
            Debug.Log("Door unlocked with key!");

            if (destroyKeyOnUse)
            {
                // Zničení předmětu přes tvou hotovou funkci v inventáři
                PlayerInventory.Instance.ConsumeCurrentItem();
                Debug.Log("Key destroyed: " + requiredKeyName);
            }

            ToggleDoor();
        }
        else
        {
            Debug.Log("The door is locked. You need to hold: " + requiredKeyName);
        }
    }

    // Tuto funkci zavolá Keypad
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door unlocked via keypad!");
        ToggleDoor(); 
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        // Zde v budoucnu spustíš animaci přes Animator
        Debug.Log(isOpen ? "Door opened." : "Door closed.");
    }
}