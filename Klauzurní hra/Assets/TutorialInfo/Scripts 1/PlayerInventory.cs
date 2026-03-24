using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Nastavení")]
    public Transform handPosition; 
    
    [Header("Animace (Nesahej do Animatoru)")]
    public Animator playerAnimator; // Přetáhni hráče
    public int armLayerIndex = 1; // Vrstva 1 je většinou ta tvoje maska (0 je Base Layer)

    private PickupItem[] slots = new PickupItem[3];
    private int currentSlot = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        // Přepínání čísel a kolečka...
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchSlot(2);

        if (Mouse.current != null)
        {
            float scrollY = Mouse.current.scroll.ReadValue().y;
            if (scrollY > 0)
            {
                int nextSlot = currentSlot - 1;
                if (nextSlot < 0) nextSlot = slots.Length - 1; 
                SwitchSlot(nextSlot);
            }
            else if (scrollY < 0)
            {
                int nextSlot = currentSlot + 1;
                if (nextSlot >= slots.Length) nextSlot = 0; 
                SwitchSlot(nextSlot);
            }
        }

        if (Keyboard.current.gKey.wasPressedThisFrame) DropItem();
    }

    public void TryPickup(PickupItem item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item; 
                item.rb.isKinematic = true;
                item.coll.enabled = false;

                item.transform.SetParent(handPosition);
                item.transform.localPosition = Vector3.zero; 
                item.transform.localRotation = Quaternion.identity; 

                SwitchSlot(i); 
                return;
            }
        }
        Debug.Log("Inventář je plný!");
    }

    public void SwitchSlot(int index)
        {
            currentSlot = index;
            bool hasItemInHand = false; 

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    bool isActive = (i == currentSlot);
                    slots[i].gameObject.SetActive(isActive);
                    
                    if (isActive) hasItemInHand = true; 
                }
            }

            // --- ŘEKNE ANIMATORU, JESTLI MÁ ZVEDNOUT RUKU ---
            if (playerAnimator != null)
            {
                // Pokud držíme = 1 (zapnuto), pokud ne = 0 (vypnuto)
                float targetWeight = hasItemInHand ? 1f : 0f;
                playerAnimator.SetLayerWeight(armLayerIndex, targetWeight);
                
                // --- DIAGNOSTIKA ---
                string itemName = hasItemInHand ? slots[currentSlot].name : "PRÁZDNO";
                Debug.Log($"<b>[DIAG] Inventory:</b> Slot {index}, Věc v ruce: {itemName}, Váha vrstvy {armLayerIndex} nastavena na: {targetWeight}");
                // --- KONEC DIAGNOSTIKY ---
            } else {
                Debug.LogError("<b>[DIAG] CHYBA:</b> Políčko 'Player Animator' v PlayerInventory je prázdné (None)! Ruka se nemůže zvednout.");
            }
        }

    public void DropItem()
    {
        PickupItem itemToDrop = slots[currentSlot];
        
        if (itemToDrop != null)
        {
            itemToDrop.transform.SetParent(null);
            itemToDrop.transform.position = handPosition.position + handPosition.forward * 0.2f;
            itemToDrop.gameObject.SetActive(true);
            itemToDrop.rb.isKinematic = false;
            itemToDrop.coll.enabled = true;
            itemToDrop.rb.AddForce(handPosition.forward * 2f, ForceMode.Impulse);

            slots[currentSlot] = null;

            // --- Když vyhodíme poslední věc, ruka jde dolů ---
            if (playerAnimator != null)
            {
                playerAnimator.SetLayerWeight(armLayerIndex, 0f);
            }
        }
    }

    // Přidaná metoda pro kontrolu drženého předmětu
    public string GetCurrentItemName()
    {
        if (slots[currentSlot] != null) 
            return slots[currentSlot].gameObject.name;
        
        return "";
    }
}