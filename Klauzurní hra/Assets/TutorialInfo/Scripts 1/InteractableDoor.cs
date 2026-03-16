using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 3f;
    
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, openAngle, 0));
    }

    void Update()
    {
        Quaternion target = isOpen ? openRotation : closedRotation;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * speed);
    }

    // Tuto funkci zavolá hráč
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}