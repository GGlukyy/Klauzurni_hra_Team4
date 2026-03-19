using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Nastavení Interakce")]
    public float interactRange = 3f;
    public LayerMask interactableLayers; // V Inspectoru nastav na vrstvu "Interactable"

    [Header("Input Akce")]
    public InputActionReference interactAction; // Levé tlačítko myši (nebo E)
    public InputActionReference lookAction;     // Pohyb myši (čte Mouse Delta)

    [Header("Bonus: Ruka")]
    public Animator handAnimator; // Zbraň nebo ruka

    private PhasmaDoor grabbedDoor = null;

    private void Start()
    {
        // DŮLEŽITÉ: Bez tohoto ti nový Input System nebude fungovat!
        if (interactAction != null) interactAction.action.Enable();
        if (lookAction != null) lookAction.action.Enable();
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.started += StartInteract;
            interactAction.action.canceled += StopInteract;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.started -= StartInteract;
            interactAction.action.canceled -= StopInteract;
        }
    }

        private void StartInteract(InputAction.CallbackContext ctx)
    {
        // KROK 1: Zjistíme, jestli vůbec funguje Input (zmáčknutí klávesy)
        Debug.Log("1. Input funguje: Zmáčkl jsi klávesu!");

        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel.activeSelf) return;

        Ray ray = new Ray(transform.position, transform.forward);
        
        // Vykreslí viditelný červený paprsek ve Scene view (zmizí po 3 vteřinách)
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red, 3f);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayers))
        {
            // KROK 2: Paprsek trefil vrstvu Interactable
            Debug.Log("<color=yellow>2. Paprsek narazil do: " + hit.collider.gameObject.name + "</color>");

            PhasmaDoor door = hit.collider.GetComponent<PhasmaDoor>();
            if (door != null)
            {
                Debug.Log("<color=green>3. ÚSPĚCH: Chytil jsi dveře!</color>");
                grabbedDoor = door;
                return;
            }

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("<color=green>3. ÚSPĚCH: Spuštěna interakce (NPC)!</color>");
                interactable.Interact();
                
                if (handAnimator != null) handAnimator.SetTrigger("Interact");
            }
            else
            {
                // KROK 3: Trefili jsme objekt, ale nemá na sobě správný skript
                Debug.Log("<color=red>3. CHYBA: Objekt má sice vrstvu Interactable, ale chybí mu skript PhasmaDoor nebo IInteractable!</color>");
            }
        }
        else
        {
            Debug.Log("<color=orange>2. Paprsek letí do prázdna (nebo objekt nemá vrstvu Interactable).</color>");
        }
    }
    private void StopInteract(InputAction.CallbackContext ctx)
    {
        // Když hráč pustí tlačítko interakce, pustíme dveře
        grabbedDoor = null; 
    }

    private void Update()
    {
        // 3. Fyzikální tahání za dveře
        if (grabbedDoor != null && lookAction != null)
        {
            // Přečteme pohyb myši na ose X (doleva / doprava)
            float mouseX = lookAction.action.ReadValue<Vector2>().x;
            grabbedDoor.PullDoor(mouseX);
        }
    }
}