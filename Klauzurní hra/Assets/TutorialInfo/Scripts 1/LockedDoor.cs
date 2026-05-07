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
    public bool requiresCode = false; // Toto zaškrtneš u dveří v prvním levelu

    public void Interact()
    {
        // 1. Pokud jsou dveře už odemčené, prostě je otevřeme/zavřeme
        if (!isLocked)
        {
            ToggleDoor();
            return;
        }

        // 2. Pokud jsou dveře na KÓD, ignorujeme klíče
        if (requiresCode)
        {
            Debug.Log("This door is locked electronically. You need a code.");
            return; // Ukončí to funkci, takže to dál nehledá klíč
        }

        // 3. PŮVODNÍ LOGIKA PRO KLÍČ
        // Zde si vlož svou původní kontrolu, jestli má hráč klíč (z inventáře nebo z ruky)
        // Tohle je ukázka, jak to zhruba funguje:
        
        bool hasKey = false; // NAHRAĎ SVOU VLASTNÍ PODMÍNKOU (např. inventory.HasItem(requiredKeyName))

        if (hasKey)
        {
            isLocked = false;
            Debug.Log("Door unlocked with key!");

            if (destroyKeyOnUse)
            {
                // Zde dej svůj kód pro zničení klíče
                Debug.Log("Key destroyed: " + requiredKeyName);
            }

            ToggleDoor();
        }
        else
        {
            Debug.Log("The door is locked. You need: " + requiredKeyName);
        }
    }

    // TUTO FUNKCI ZAVOLÁ TVŮJ KEYPAD MANAGER PŘES INSPECTOR
    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log("Door unlocked via keypad!");
        
        // Pokud chceš, aby se dveře hned i otevřely, když zadáš kód, odkomentuj řádek pod tímto:
        // ToggleDoor();
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        // Zde si dej svou původní logiku otevírání/zavírání (např. spuštění animace)
        Debug.Log(isOpen ? "Door opened." : "Door closed.");
    }
}