using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DarkZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SanityManager.Instance.SetDarkness(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SanityManager.Instance.SetDarkness(false);
        }
    }
}