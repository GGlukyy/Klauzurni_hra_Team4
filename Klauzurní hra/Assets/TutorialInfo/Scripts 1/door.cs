using UnityEngine;

public class door : MonoBehaviour
{
    public float smooth = 3f;
    public float DoorOpenAngle = 90f;

    private bool open = false;
    private bool enter = false;

    private Quaternion defaultRot;
    private Quaternion openRot;

    void Start()
    {
        // Uložení výchozí a cílové rotace
        defaultRot = transform.localRotation;
        openRot = Quaternion.Euler(defaultRot.eulerAngles + new Vector3(0, DoorOpenAngle, 0));
    }

    void Update()
    {
        // Kontrola stisknutí klávesy F, POUZE pokud je hráč v Trigger zóně
        if (enter && Input.GetKeyDown(KeyCode.F))
        {
            open = !open;
        }

        // Plynulá rotace otevírání/zavírání
        Quaternion targetRotation = open ? openRot : defaultRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
    }

    // Detekce, že hráč VEŠEL do zóny
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            enter = true;
        }
    }

    // Detekce, že hráč ODEŠEL ze zóny
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enter = false;
        }
    }
}