using UnityEngine;

// Přidali jsme IInteractable
public class NPCInteract : MonoBehaviour, IInteractable 
{
    [TextArea(3, 10)]
    public string[] lines;

    // Tuto funkci teď zavolá hráčův paprsek
    public void Interact()
    {
        DialogueManager.Instance.StartDialogue(lines);
    }
}