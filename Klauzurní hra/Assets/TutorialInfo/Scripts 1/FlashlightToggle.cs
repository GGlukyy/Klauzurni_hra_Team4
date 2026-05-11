using UnityEngine;
using UnityEngine.InputSystem; // Nutné pro New Input System

[RequireComponent(typeof(Light))]
public class FlashlightToggle : MonoBehaviour
{
    private Light flashlight;
    
    [Header("Nastavení")]
    [Tooltip("Určuje, zda je baterka na začátku zapnutá.")]
    public bool isOn = false; 

    private AudioSource clickSound;

    void Start()
    {
        flashlight = GetComponent<Light>();
        clickSound = GetComponent<AudioSource>();
        
        flashlight.enabled = isOn;
    }

    void Update()
    {
        // Kontrola, zda existuje myš a zda bylo stisknuto levé tlačítko
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) 
        {
            ToggleFlashlight();
        }

        // Pokud bys chtěl baterku na klávesu 'F' místo myši, použij toto:
        // if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame) { ToggleFlashlight(); }
    }

    private void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlight.enabled = isOn;

        if (clickSound != null)
        {
            clickSound.Play();
        }
    }
}