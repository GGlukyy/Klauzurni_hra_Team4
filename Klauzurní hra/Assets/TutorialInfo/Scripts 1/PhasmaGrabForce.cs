using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhasmaGrabForce : MonoBehaviour
{
    private SpringJoint grabJoint;

    public void Grab(Rigidbody handRigidbody, Vector3 grabPointWorld)
    {
        if (grabJoint != null) return; // Už držíme

        // Přidáme fyzikální pružinu
        grabJoint = gameObject.AddComponent<SpringJoint>();
        grabJoint.connectedBody = handRigidbody;
        
        // Připneme pružinu přesně na to místo na dveřích, kam jsi klikl
        grabJoint.anchor = transform.InverseTransformPoint(grabPointWorld);
        
        // Druhý konec pružiny drží tvá neviditelná ruka
        grabJoint.autoConfigureConnectedAnchor = false;
        grabJoint.connectedAnchor = Vector3.zero;

        // Fyzikální síla tahu (můžeš si s tím pak pohrát)
        grabJoint.spring = 500f;   // Síla, jakou taháš
        grabJoint.damper = 50f;     // Tlumič, aby dveře nekmitaly
        grabJoint.maxDistance = 0f; // Chceme, aby bod následoval ruku přesně
    }

    public void Release()
    {
        if (grabJoint == null) return;
        Destroy(grabJoint);
        grabJoint = null;
    }
}