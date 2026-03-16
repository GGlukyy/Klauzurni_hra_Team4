using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Nastavení Interakce")]
    public float interactRange = 3f;
    public InputActionReference interactAction; // Vytvoř si v Input Actions tlačítko (např. "E")

    [Header("Bonus: Ruka")]
    public Animator handAnimator; // Sem přetáhni Animator tvé ruky/zbraně

    private void OnEnable() => interactAction.action.performed += TryInteract;
    private void OnDisable() => interactAction.action.performed -= TryInteract;

    private void TryInteract(InputAction.CallbackContext ctx)
    {
        // Vystřelí paprsek ze středu kamery směrem vpřed
        Ray ray = new Ray(transform.position, transform.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            // Zkusí najít komponentu Door na objektu, do kterého jsme narazili
            Door door = hit.collider.GetComponent<Door>();
            
            if (door != null)
            {
                door.ToggleDoor();
                
                // Zde spouštíme bonusovou animaci
                if (handAnimator != null)
                {
                    handAnimator.SetTrigger("Interact");
                }
            }
        }
    }
}