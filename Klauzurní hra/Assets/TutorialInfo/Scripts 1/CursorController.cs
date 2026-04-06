using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [Header("Nastavení Myši")]
    public float mouseSensitivity = 10f;
    
    [Header("Odkazy")]
    public Transform playerBody; // Sem přetáhneme kapsli (Player)
    public Transform cameraTransform; // Sem přetáhneme Main Camera

    private float xRotation = 0f;

    private void Awake()
    {
        // Zamkne a schová myš
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Čtení pohybu myši přímo z nového Input Systému
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Výpočet rotace nahoru a dolů (pitch)
        xRotation -= mouseY;
        // Omezíme úhel, aby si hráč nezlomil krk a nepodíval se sám sobě do zad
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

        // 1. Aplikujeme naklánění nahoru/dolů POUZE na kameru
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 2. Aplikujeme otáčení doleva/doprava na CELOU KAPSLI (hráče)
        playerBody.Rotate(Vector3.up * mouseX);
    }
    private void OnEnable()
    {
        // Načte aktuální rotaci kamery, aby se xRotation neresetovala na 0
        Vector3 rotation = cameraTransform.localEulerAngles;
        xRotation = rotation.x > 180 ? rotation.x - 360 : rotation.x;
    }
}