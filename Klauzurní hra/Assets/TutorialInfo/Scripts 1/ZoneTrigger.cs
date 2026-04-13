using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public enum ZoneType { CrouchZone, DeathZone }
    public ZoneType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PrologueManager manager = FindObjectOfType<PrologueManager>();
            if (manager != null)
            {
                if (type == ZoneType.CrouchZone) manager.hasPassedCrouchZone = true;
                if (type == ZoneType.DeathZone) manager.isInDeathZone = true;
            }
        }
    }
}