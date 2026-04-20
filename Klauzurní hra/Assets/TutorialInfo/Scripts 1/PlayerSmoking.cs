using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSmoking : MonoBehaviour
{
    [Header("Nastavení kouření")]
    public float healAmount = 20f;
    public InputActionReference smokeAction; // Nastav na klávesu (např. C)

    private void OnEnable()
    {
        smokeAction.action.performed += SmokeCigarette;
        smokeAction.action.Enable();
    }

    private void OnDisable()
    {
        smokeAction.action.performed -= SmokeCigarette;
    }

    private void SmokeCigarette(InputAction.CallbackContext ctx)
    {
        if (SanityManager.Instance.cigarettesInInventory > 0)
        {
            SanityManager.Instance.cigarettesInInventory--;
            SanityManager.Instance.HealSanity(healAmount);
            
            Debug.Log("Zapaluješ si cigaretu... Sanity se lepší. Zbývá: " + SanityManager.Instance.cigarettesInInventory);
            // Tady můžeš spustit animaci ruky s cigaretou nebo zvuk zapalovače
        }
        else
        {
            Debug.Log("Nemáš žádné cigarety!");
        }
    }
}