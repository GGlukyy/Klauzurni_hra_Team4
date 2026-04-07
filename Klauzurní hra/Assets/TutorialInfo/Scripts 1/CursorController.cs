using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [Header("Nastavení Myši")]
    public float mouseSensitivity = 10f;
    
    [Header("Odkazy")]
    public Transform playerBody; 
    public Transform cameraTransform; 

    [Header("Cutscéna (Gumičkový efekt)")]
    public bool inCutscene = false;
    public float cutsceneSensitivity = 5f;
    public float maxAngleX = 20f; // Jak moc se může podívat do stran
    public float maxAngleY = 15f; // Jak moc se může podívat nahoru/dolu
    public float returnSpeed = 4f; // Jak rychle se to vrátí na střed

    private float xRotation = 0f;
    private Vector2 cutsceneLookOffset;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 1. POKUD JSME V CUTSCÉNĚ, ZABLOKUJEME NORMÁLNÍ TOČENÍ
        if (inCutscene) return;

        // 2. NORMÁLNÍ ROZHLÍŽENÍ VE HŘE
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LateUpdate()
    {
        // 3. GUMIČKOVÝ EFEKT BĚHEM CUTSCÉNY
        // Musí být v LateUpdate, aby naši rotaci nepřepsala animace hlavy
        if (!inCutscene) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * cutsceneSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * cutsceneSensitivity * Time.deltaTime;

        // Omezení, kam až může myší pohnout
        cutsceneLookOffset.x = Mathf.Clamp(cutsceneLookOffset.x + mouseX, -maxAngleX, maxAngleX);
        cutsceneLookOffset.y = Mathf.Clamp(cutsceneLookOffset.y - mouseY, -maxAngleY, maxAngleY);

        // Návrat na střed (gumička)
        cutsceneLookOffset = Vector2.Lerp(cutsceneLookOffset, Vector2.zero, Time.deltaTime * returnSpeed);

        // Aplikujeme tento malý výkyv na kameru
        cameraTransform.localRotation *= Quaternion.Euler(-cutsceneLookOffset.y, cutsceneLookOffset.x, 0f);
    }

    private void OnEnable()
    {
        Vector3 rotation = cameraTransform.localEulerAngles;
        xRotation = rotation.x > 180 ? rotation.x - 360 : rotation.x;
    }
}