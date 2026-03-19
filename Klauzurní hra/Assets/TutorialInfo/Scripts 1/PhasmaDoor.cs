using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhasmaDoor : MonoBehaviour
{
    private Rigidbody rb;
    public float doorSensitivity = 2f; // Jak moc dveře reagují na pohyb myši

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Tuto funkci volá hráčův skript každý frame, kdy DRŽÍ tlačítko
    public void PullDoor(float mouseMoveX)
    {
        // Přidáme točivý moment (Torque) na ose Y podle toho, jak se hýbe myš
        rb.AddRelativeTorque(Vector3.up * mouseMoveX * doorSensitivity, ForceMode.VelocityChange);
    }
}