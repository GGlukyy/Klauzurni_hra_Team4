using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSMovement : MonoBehaviour
{
    [Header("Speed Values")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;

    [Header("References")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference crouchAction;
    public InputActionReference sprintAction;
    public Transform cameraTransform;

    [Header("Gravity & Jumping")]
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float initialFallVelocity = -2f;

    [Header("Crouching")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;
    public float crouchTransitionSpeed = 10f;
    public float cameraOffset = 0.2f;

    private CharacterController characterController;
    private Vector2 moveInput;
    private bool isGrounded;
    private float verticalVelocity;
    
    private bool isRunning;
    private bool isCrouching;
    private float targetHeight;
    private bool isCrouchButtonPressed; 

    // --- PŘIDÁNO PRO ANIMACE ---
    [Header("Audio & Animations")]
    public Animator animator;
    public AudioSource footstepSource;
    public AudioClip[] footstepSounds;
    // ---------------------------

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        targetHeight = standingHeight;
        
        // --- PŘIDÁNO PRO ANIMACE ---
        // Pokud jsi zapomněl přiřadit Animator v Inspectoru, zkusí si ho najít sám
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        // ---------------------------
    }

    private void OnEnable()
    {
        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;

        jumpAction.action.performed += Jump;
        
        crouchAction.action.performed += HandleCrouchInput;
        crouchAction.action.canceled += HandleCrouchInput;
        
        sprintAction.action.performed += ToggleSprint;
        sprintAction.action.canceled += ToggleSprint;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;

        jumpAction.action.performed -= Jump;
        
        crouchAction.action.performed -= HandleCrouchInput;
        crouchAction.action.canceled -= HandleCrouchInput;
        
        sprintAction.action.performed -= ToggleSprint;
        sprintAction.action.canceled -= ToggleSprint;
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;

        // Logika pro Hold to Crouch
        if (isCrouchButtonPressed && !isCrouching)
        {
            isCrouching = true;
            targetHeight = crouchingHeight;
        }
        else if (!isCrouchButtonPressed && isCrouching && CanStandUp())
        {
            isCrouching = false;
            targetHeight = standingHeight;
        }

        HandleGravity();
        HandleMovement();
        HandleCrouchTransition();
        
        // --- PŘIDÁNO PRO OTÁČENÍ TĚLA ---
        // Kapsle se natočí přesně tam, kam se dívá kamera (pouze horizontálně)

        // --------------------------------
        
        UpdateAnimations(); 
    }

    private void StoreMovementInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        // Přidáno "&& !isCrouching" - Hráč musí být na zemi A ZÁROVEŇ nesmí dřepět
        if (isGrounded && !isCrouching) 
        {
            verticalVelocity = jumpForce;
            
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    private void HandleCrouchInput(InputAction.CallbackContext context)
    {
        isCrouchButtonPressed = context.performed;
    }

    private void ToggleSprint(InputAction.CallbackContext context)
    {
        isRunning = context.performed;
    }

    private void HandleGravity()
    {
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = initialFallVelocity;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
        direction = cameraTransform.TransformDirection(direction);
        direction.y = 0f;
        direction.Normalize();

        float currentSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
        
        Vector3 finalMove = direction * currentSpeed;
        finalMove.y = verticalVelocity;

        CollisionFlags collisions = characterController.Move(finalMove * Time.deltaTime);
        if ((collisions & CollisionFlags.Above) != 0)
        {
            verticalVelocity = initialFallVelocity;
        }
    }

    private void HandleCrouchTransition()
    {
        float currentHeight = characterController.height;
        
        float newHeight = Mathf.Lerp(currentHeight, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        characterController.height = newHeight;
        characterController.center = new Vector3(0, newHeight / 2f, 0);

        Vector3 camPos = cameraTransform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetHeight - cameraOffset, crouchTransitionSpeed * Time.deltaTime);
        cameraTransform.localPosition = camPos;
    }

    private bool CanStandUp()
    {
        float radius = characterController.radius;
        Vector3 p1 = transform.position + new Vector3(0, radius, 0);
        Vector3 p2 = transform.position + new Vector3(0, standingHeight - radius, 0);
        
        return !Physics.CapsuleCast(p1, p2, radius, Vector3.up, out RaycastHit hit, 0.1f);
    }
    
    // --- PŘIDÁNO PRO ANIMACE ---
        private void UpdateAnimations()
            {
                if (animator == null) return;

                float inputMagnitude = moveInput.magnitude;
                float animSpeed = 0f;

                if (inputMagnitude > 0.1f) 
                {
                    if (isRunning && !isCrouching) 
                    {
                        animSpeed = 1f; 
                    }
                    else 
                    {
                        animSpeed = 0.5f; // Chůze (nebo chůze ve dřepu)
                    }
                }

                float currentAnimSpeed = animator.GetFloat("Speed");
                float smoothAnimSpeed = Mathf.Lerp(currentAnimSpeed, animSpeed, Time.deltaTime * 10f);

                animator.SetFloat("Speed", smoothAnimSpeed);

                // --- PŘIDÁNO PRO DŘEP ---
                // Pošle do Animátoru true/false podle toho, jestli hráč zrovna drží tlačítko dřepu
                animator.SetBool("IsCrouching", isCrouching);
                // ------------------------
                // --- PŘIDÁNO PRO SKOK ---
                // Dává Animátoru vědět, jestli zrovna stojíme pevně na zemi
                animator.SetBool("IsGrounded", isGrounded);
                // ------------------------
            }

    public void PlayFootstepSound()
    {
        if (isGrounded && footstepSounds.Length > 0)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            footstepSource.PlayOneShot(clip, Random.Range(0.8f, 1f));
        }
    }
    // ---------------------------
}