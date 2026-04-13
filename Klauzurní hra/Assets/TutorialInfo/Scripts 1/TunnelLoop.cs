using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TunnelLoop : MonoBehaviour
{
    [Tooltip("Prázdný GameObject reprezentující druhou stranu tunelu.")]
    public Transform destination;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();

            // Vypočítáme přesnou pozici hráče vůči tomuto triggeru, aby teleport nebyl trhaný
            Vector3 offset = other.transform.position - transform.position;

            // CharacterController se musí před manuální změnou pozice vypnout
            if (cc != null) cc.enabled = false;

            // Teleport hráče na cíl + zachování jeho odchylky od středu
            other.transform.position = destination.position + offset;

            // Znovu zapneme pohyb
            if (cc != null) cc.enabled = true;
        }
    }
}