using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // DŮLEŽITÉ: Přidán nový Input System

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.05f;
    public AudioSource typingSound; 

    private bool isTyping = false;
    private bool cancelTyping = false; 

    private void Awake() => Instance = this;

    public void StartDialogue(string[] lines)
    {
        if (!dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(TypeLines(lines));
        }
    }

    private IEnumerator TypeLines(string[] lines)
    {
        foreach (string line in lines)
        {
            isTyping = true;
            cancelTyping = false;
            dialogueText.text = "";

            foreach (char c in line.ToCharArray())
            {
                if (cancelTyping) 
                {
                    dialogueText.text = line; 
                    break;
                }

                dialogueText.text += c;
                if (typingSound != null) typingSound.Play(); 
                
                yield return new WaitForSeconds(typingSpeed);
            }

            isTyping = false;

            // Čeká na zmáčknutí Levé myši nebo klávesy E přes nový Input System
            yield return new WaitUntil(() => 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            );
            
            yield return null; 
        }
        
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // Detekce stisknutí pro přeskočení pomalého psaní
        bool isPressed = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                         (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame);

        if (isTyping && isPressed)
        {
            cancelTyping = true;
        }
    }
}