using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PrologueManager : MonoBehaviour
{
    [Header("Hlavní objekty")]
    public Transform playerCamera;
    public MonoBehaviour playerMovement; // Tady doplň svůj skript na pohyb (např. FirstPersonController)
    public NPCFriend friend;
    public PlayableDirector introCutscene; // Timeline pro začátek
    public PlayableDirector trainCutscene; // Timeline pro finále s vlakem

    [Header("Navigační body")]
    public Transform crouchPoint;
    public Transform finalWaitPoint;

    [Header("Světla a Cíle kamery")]
    public Light flashingLight; // Světlo, co začne blikat
    public Transform guardLightTransform; // Bod, kde se objeví světlo hlídače
    public GameObject guardLightObject; 

    [Header("Dabing")]
    public AudioClip wowHusty;
    public AudioClip bachaNekdoJde;
    public AudioClip tedTySkrceni;
    public AudioClip vemBaterku;

    [Header("Stavy hráče (Mění se z interakcí)")]
    public bool hasPickedUpSpray = false;
    public bool hasSprayedWall = false;
    public bool hasPassedCrouchZone = false;
    public bool hasPickedUpFlashlight = false;
    public bool isInDeathZone = false;

    private void Start()
    {
        StartCoroutine(PrologueSequence());
    }

    private IEnumerator PrologueSequence()
    {
        // 1. ÚVODNÍ CUTSCÉNA (Kámoš sprejuje)
        playerMovement.enabled = false;
        introCutscene.Play();
        yield return new WaitUntil(() => introCutscene.state != PlayState.Playing); // Čeká na konec Timeliny
        playerMovement.enabled = true;

        // 2. HRÁČŮV ÚKOL (Vzít sprej a nasprejovat)
        yield return new WaitUntil(() => hasPickedUpSpray && hasSprayedWall);

        // 3. LOCK NA KAMARÁDA: "Wow hustý"
        playerMovement.enabled = false;
        yield return StartCoroutine(SmoothLookAt(friend.transform.position, 0.8f));
        friend.Say(wowHusty);
        yield return new WaitForSeconds(2f);
        playerMovement.enabled = true;

        // 4. BLIKÁNÍ SVĚTLA (Náznak nebezpečí)
        yield return StartCoroutine(BlinkLight(flashingLight, 3f));

        // 5. LOCK NA SVĚTLO A "Bacha někdo jde!"
        playerMovement.enabled = false;
        guardLightObject.SetActive(true);
        yield return StartCoroutine(SmoothLookAt(guardLightTransform.position, 0.5f));
        friend.Say(bachaNekdoJde);
        yield return new WaitForSeconds(1.5f);
        playerMovement.enabled = true;

        // 6. ÚTĚK K POTRUBÍ (S otravnými hláškami)
        friend.StartAnnoyingShouts();
        friend.MoveToPoint(crouchPoint, 5f);
        yield return new WaitUntil(() => friend.HasReachedDestination());
        friend.StopAnnoyingShouts();

        // 7. KAMARÁD PROLÉZÁ A ČEKÁ
        friend.anim.SetTrigger("Crawl"); // Animace prolézání kámoše
        yield return new WaitForSeconds(2f); // Čas než animace skončí
        friend.Say(tedTySkrceni);

        // 8. ČEKÁNÍ NA HRÁČE (Musí prolézt colliderem)
        yield return new WaitUntil(() => hasPassedCrouchZone);

        // 9. LOCK NA KAMARÁDA ("Vem baterku")
        playerMovement.enabled = false;
        yield return StartCoroutine(SmoothLookAt(friend.transform.position, 0.6f));
        friend.Say(vemBaterku);
        yield return new WaitForSeconds(1.5f);
        playerMovement.enabled = true;

        // 10. SEBRÁNÍ BATERKY
        yield return new WaitUntil(() => hasPickedUpFlashlight);

        // 11. JEMNÝ LOCK NA SVĚTLO A ZDRHÁNÍ
        playerMovement.enabled = false;
        yield return StartCoroutine(SmoothLookAt(guardLightTransform.position, 0.4f));
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(SmoothLookAt(friend.transform.position, 0.4f));
        playerMovement.enabled = true;

        // 12. FINÁLNÍ SPRINT
        friend.StartAnnoyingShouts();
        friend.MoveToPoint(finalWaitPoint, 6.5f); // Zrychlí
        yield return new WaitUntil(() => friend.HasReachedDestination());
        friend.StopAnnoyingShouts();

        // 13. ČEKÁNÍ PŘED ZATÁČKOU (Pobízení)
        friend.anim.SetBool("IsBeckoning", true); // Přehrává animaci "Pojď dělej" ve smyčce
        
        yield return new WaitUntil(() => isInDeathZone);

        // 14. VLAK SMRT (Konec levelu)
        friend.anim.SetBool("IsBeckoning", false);
        playerMovement.enabled = false;
        trainCutscene.Play(); // Spustí epickou animaci vlaku a tmu
    }

    // Funkce na lockování kamery
    private IEnumerator SmoothLookAt(Vector3 targetPosition, float duration)
    {
        Quaternion startRot = playerCamera.rotation;
        Vector3 direction = (targetPosition - playerCamera.position).normalized;
        // Ignorujeme osu Y, aby kamera nekoukala do země, pokud je bod níž
        direction.y = playerCamera.forward.y; 
        Quaternion targetRot = Quaternion.LookRotation(direction);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
    }

    // Pomocný efekt blikání
    private IEnumerator BlinkLight(Light lightComponent, float duration)
    {
        if (lightComponent == null) yield break;
        
        float timer = 0;
        float originalIntensity = lightComponent.intensity;
        
        while (timer < duration)
        {
            lightComponent.intensity = Random.Range(0f, originalIntensity);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            timer += 0.2f;
        }
        lightComponent.intensity = 0; // Světlo úplně zhasne
    }
}