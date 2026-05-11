using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        // Najde hlavní kameru (hráče)
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        // Otáčí text tak, aby vždy koukal přesně proti kameře
        transform.forward = mainCameraTransform.forward;
    }
}