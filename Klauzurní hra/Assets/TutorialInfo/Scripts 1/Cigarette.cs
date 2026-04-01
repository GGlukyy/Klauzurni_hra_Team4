using UnityEngine;

public class Cigarette : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SanityManager.Instance.cigarettesInInventory++;
        Debug.Log("Sebral jsi cigaretu. Celkem: " + SanityManager.Instance.cigarettesInInventory);
        Destroy(gameObject);
    }
}