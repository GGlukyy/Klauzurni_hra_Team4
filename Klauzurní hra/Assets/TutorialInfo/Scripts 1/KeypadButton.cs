using UnityEngine;

public class KeypadButton : MonoBehaviour, IInteractable
{
    [Header("Setup")]
    public KeypadManager manager;
    
    [Tooltip("Type a number (0-9), 'ENTER', or 'CLEAR'")]
    public string buttonValue;

    // Funkce z tvého IInteractable interface
    public void Interact()
    {
        if (manager == null) return;

        // Rozhodne, co se má stát podle hodnoty tlačítka
        if (buttonValue == "ENTER")
        {
            manager.SubmitCode();
        }
        else if (buttonValue == "CLEAR")
        {
            manager.ClearCode();
        }
        else
        {
            manager.AddDigit(buttonValue);
        }
    }
}