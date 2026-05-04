using UnityEngine;
using TMPro; // Potřebné pro 3D text
using UnityEngine.Events;
using System.Collections;

public class KeypadManager : MonoBehaviour
{
    [Header("Settings")]
    public string password = "734";
    public TextMeshPro displayText;

    [Header("Events")]
    public UnityEvent onUnlocked;

    private string currentInput = "";
    private bool isLockedOut = false;

    private void Start()
    {
        UpdateDisplay();
    }

    // Přidá číslo na displej
    public void AddDigit(string digit)
    {
        if (isLockedOut || currentInput.Length >= password.Length) return;

        currentInput += digit;
        UpdateDisplay();
    }

    // Tlačítko ENTER
    public void SubmitCode()
    {
        if (isLockedOut || currentInput.Length == 0) return;

        if (currentInput == password)
        {
            displayText.text = "OK";
            isLockedOut = true; // Zablokuje další mačkání
            onUnlocked.Invoke(); // Spustí akci (otevření dveří)
        }
        else
        {
            StartCoroutine(ShowErrorRoutine());
        }
    }

    // Tlačítko CLEAR
    public void ClearCode()
    {
        if (isLockedOut) return;
        currentInput = "";
        UpdateDisplay();
    }

    // Aktualizuje text na displeji (např. nahradí prázdná místa pomlčkou)
    private void UpdateDisplay()
    {
        displayText.text = currentInput.PadRight(password.Length, '-');
    }

    // Zobrazí ERR, chvíli počká a vymaže displej
    private IEnumerator ShowErrorRoutine()
    {
        isLockedOut = true;
        displayText.text = "ERR";
        
        yield return new WaitForSeconds(1.5f); // Doba zobrazení ERR
        
        ClearCode();
        isLockedOut = false;
    }
}