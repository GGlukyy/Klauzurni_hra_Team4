using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // NOVÉ: Potřebujeme pro práci s UI (Image)

public class PlayerInteract : MonoBehaviour
{
    [Header("Nastavení Interakce")]
    public float interactRange = 3f;
    
    [Tooltip("POZOR: Zde musí být zaškrtnuté OBĚ vrstvy - Interactable i ta tvoje Outline!")]
    public LayerMask interactableLayers; 

    [Header("Outline Nastavení")]
    [Tooltip("Číslo vrstvy, která dělá tvůj Outline efekt (např. 8)")]
    public int outlineLayerIndex; 

    [Header("UI Kurzor (Zámek)")] // --- NOVÉ ---
    public Image crosshairImage; // Obrázek kurzoru uprostřed obrazovky
    public Sprite normalCursorSprite; // Základní tečka/kurzor
    public Sprite lockedCursorSprite; // Ikonka zámku

    [Header("Input Akce")]
    public InputActionReference interactAction; 

    [Header("References")]
    public Camera mainCam;

    private PhasmaGrabForce grabbedDoor = null;
    private GameObject physicsHand; 
    private Rigidbody handRb;
    private float grabDistance; 

    private GameObject currentLookTarget;
    private int originalLayer;

    private void Awake()
    {
        if (mainCam == null) mainCam = Camera.main;

        physicsHand = new GameObject("GrabHand");
        handRb = physicsHand.AddComponent<Rigidbody>();
        handRb.isKinematic = true; 
        handRb.useGravity = false;
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.started += StartGrab;
            interactAction.action.canceled += StopGrab;
            interactAction.action.Enable(); 
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.started -= StartGrab;
            interactAction.action.canceled -= StopGrab;
        }
    }

    private void StartGrab(InputAction.CallbackContext ctx)
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.dialoguePanel.activeSelf) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayers))
        {
            // 1. NEJPRVE KONTROLA ZÁMKU
            LockedDoor lockedDoor = hit.collider.GetComponent<LockedDoor>();
            if (lockedDoor != null && lockedDoor.isLocked)
            {
                lockedDoor.Interact(); 
                return; 
            }

            // 2. TAHÁNÍ DVEŘÍ
            grabbedDoor = hit.collider.GetComponent<PhasmaGrabForce>();
            if (grabbedDoor != null)
            {
                grabDistance = Vector3.Distance(mainCam.transform.position, hit.point);
                physicsHand.transform.position = hit.point; 
                grabbedDoor.Grab(handRb, hit.point);
                return;
            }

            // 3. OSTATNÍ INTERAKCE
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable is not LockedDoor) 
            {
                interactable.Interact();
            }
        }
    }

    private void StopGrab(InputAction.CallbackContext ctx)
    {
        if (grabbedDoor != null)
        {
            grabbedDoor.Release();
            grabbedDoor = null; 
        }
    }

    private void Update()
    {
        // 1. Fyzikální tahání za dveře
        if (grabbedDoor != null && physicsHand != null)
        {
            physicsHand.transform.position = mainCam.transform.position + mainCam.transform.forward * grabDistance;
        }

        // 2. OUTLINE SYSTÉM A KONTROLA KURZORU
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        bool lookingAtLockedDoor = false; // --- NOVÉ: Připravíme si proměnnou pro kurzor ---

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayers))
        {
            GameObject hitObj = hit.collider.gameObject;

            // --- NOVÉ: Zkontrolujeme, jestli se zrovna nedíváme na zamčené dveře ---
            LockedDoor lockedDoor = hitObj.GetComponent<LockedDoor>();
            if (lockedDoor != null && lockedDoor.isLocked)
            {
                lookingAtLockedDoor = true; // Zaznamenáme si, že koukáme na zámek
            }

            // Outline logika
            if (hitObj != currentLookTarget)
            {
                RemoveOutline(); 

                currentLookTarget = hitObj;
                originalLayer = currentLookTarget.layer; 
                currentLookTarget.layer = outlineLayerIndex; 
            }
        }
        else
        {
            RemoveOutline();
        }

        // --- NOVÉ: Přepnutí ikonky kurzoru ---
        if (crosshairImage != null)
        {
            // Pokud se díváme na zamčené dveře, nastav ikonku zámku, jinak základní tečku
            crosshairImage.sprite = lookingAtLockedDoor ? lockedCursorSprite : normalCursorSprite;
        }
    }

    private void RemoveOutline()
    {
        if (currentLookTarget != null)
        {
            currentLookTarget.layer = originalLayer; 
            currentLookTarget = null;
        }
    }
}