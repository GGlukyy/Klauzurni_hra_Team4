using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Escalator : MonoBehaviour
{
    [Header("Nastavení Eskalátoru")]
    [Tooltip("Rychlost pohybu eskalátoru.")]
    public float speed = 3f;
    
    [Tooltip("Zatrhni pro jízdu nahoru, odtrhni pro jízdu dolů.")]
    public bool isGoingUp = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            
            if (cc != null)
            {
                // Určíme směr (1 pro nahoru/vpřed, -1 pro dolů/vzad)
                float directionMultiplier = isGoingUp ? 1f : -1f;
                
                // Pohyb podél lokální osy Z (modrá šipka) tohoto objektu
                Vector3 moveVector = transform.forward * directionMultiplier * speed * Time.deltaTime;
                
                // Move() na CharacterControlleru zajistí, že hráč stále koliduje s okolím
                cc.Move(moveVector);
            }
        }
    }
}