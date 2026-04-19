using UnityEngine;
using UnityEngine.Playables; // Knihovna pro spouštění Timeliny (Cutscén)

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Co se má spustit")]
    public PlayableDirector deathTimeline; 

    [Header("Zablokování hráče")]
    [Tooltip("Přetáhni sem hráče, aby se mu při nárazu vypnul pohyb")]
    public FPSMovement playerMovement; 

    private bool hasTriggered = false; // Pojistka, aby se to nespustilo víckrát

    private void OnTriggerEnter(Collider other)
    {
        // Zkontrolujeme, jestli do zóny vešel opravdu "Player" a ne třeba letící předmět
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("Hráč vstoupil do pasti! Spouštím cutscénu.");

            // 1. Zmrazíme hráče (vypneme mu tvůj skript na pohyb)
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            // 2. Spustíme Timeline cutscénu (vlak vyletí ze tmy a přejede hráče)
            if (deathTimeline != null)
            {
                deathTimeline.Play();
            }
        }
    }
}