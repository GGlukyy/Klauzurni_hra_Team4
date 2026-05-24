using UnityEngine;
using UnityEngine.Playables;

public class CardScanner : MonoBehaviour
{
    [Header("Scanner Settings")]
    [Tooltip("Name of the card required to unlock")]
    public string requiredCardName = "Level1_Keycard";
    
    [Tooltip("If true, the card will be removed from the player's inventory upon success.")]
    public bool consumeCardOnUse = true;

    [Header("Cutscene Settings")]
    [Tooltip("The PlayableDirector that plays the door opening cutscene")]
    public PlayableDirector doorCutscene;

    private bool isAlreadyScanned = false;

    /// <summary>
    /// Call this from your Raycast/Interact script.
    /// You might need to pass the Player GameObject to access its inventory.
    /// </summary>
    public void TryScanCard(string heldItemName, GameObject playerObject)
    {
        if (isAlreadyScanned) return;

        if (heldItemName == requiredCardName)
        {
            UnlockAndPlayCutscene();

            if (consumeCardOnUse)
            {
                ConsumeCard(playerObject);
            }
        }
        else
        {
            Debug.Log("Access Denied: Wrong card or empty hands.");
        }
    }

    private void UnlockAndPlayCutscene()
    {
        isAlreadyScanned = true;
        Debug.Log("Access Granted! Playing cutscene...");

        if (doorCutscene != null)
        {
            doorCutscene.Play(); 
        }
        else
        {
            Debug.LogWarning("PlayableDirector is missing on the scanner!");
        }
    }

    private void ConsumeCard(GameObject playerObject)
    {
        // TODO: Propoj s tvým existujícím systémem inventáře (např. PlayerInventory.cs)
        // Příklad:
        // PlayerInventory inventory = playerObject.GetComponent<PlayerInventory>();
        // if (inventory != null)
        // {
        //     inventory.RemoveItem(requiredCardName);
        // }
        
        Debug.Log($"Card '{requiredCardName}' was consumed by the scanner.");
    }
}