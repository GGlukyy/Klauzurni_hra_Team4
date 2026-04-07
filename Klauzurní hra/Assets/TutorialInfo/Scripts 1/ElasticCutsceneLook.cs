using UnityEngine;

public class ElasticCutsceneLook : MonoBehaviour
{
    [Header("Nastavení gumičky")]
    public float sensitivity = 2f;
    public float maxAngleX = 15f; // Jak moc se může podívat do stran
    public float maxAngleY = 10f; // Jak moc se může podívat nahoru/dolu
    public float returnSpeed = 4f; // Jak rychle se to vrátí na střed

    private Vector2 lookOffset;

    void OnEnable()
    {
        lookOffset = Vector2.zero; // Reset pokaždé, když cutscéna začne
    }

    void LateUpdate()
    {
        // Předpokládám klasický input, uprav jestli používáš nový Input System na myš
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        lookOffset.x = Mathf.Clamp(lookOffset.x + mouseX, -maxAngleX, maxAngleX);
        lookOffset.y = Mathf.Clamp(lookOffset.y - mouseY, -maxAngleY, maxAngleY);

        lookOffset = Vector2.Lerp(lookOffset, Vector2.zero, Time.deltaTime * returnSpeed);

        // K animaci hlavy se přidá jen tento náš malý offset
        transform.localRotation *= Quaternion.Euler(lookOffset.y, lookOffset.x, 0f);
    }
}