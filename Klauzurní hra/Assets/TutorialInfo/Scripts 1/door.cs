using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public bool isOpen = false;
    public float doorOpenAngle = 90f;
    public float openSpeed = 2f;

    private Vector3 defaultRotation;
    private Vector3 openRotation;
    private Coroutine animationCoroutine;

    private void Start()
    {
        defaultRotation = transform.eulerAngles;
        openRotation = new Vector3(defaultRotation.x, defaultRotation.y + doorOpenAngle, defaultRotation.z);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(AnimateDoor(isOpen ? openRotation : defaultRotation));
    }

    private IEnumerator AnimateDoor(Vector3 targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float time = 0;

        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            time += Time.deltaTime * openSpeed;
            yield return null;
        }
        transform.rotation = endRotation;
    }
}