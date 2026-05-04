using System.Collections;
using UnityEngine;
using UnityEngine.Playables; // Důležité pro Timeline

public class TrainTimerEvent : MonoBehaviour
{
    [Header("Minigame Settings")]
    public float timeBetweenTrains = 20f;
    private float currentTimer;
    private bool isMinigameActive = false;
    private bool isSequenceRunning = false;

    [Header("Lights Setup")]
    public Light[] targetLights; // V inspektoru můžeš měnit velikost pole (+/-)
    public float flickerDuration = 3f;
    public float minFlickerDelay = 0.05f; // Minimální čas mezi probliknutím (pro realističtější praskání)
    public float maxFlickerDelay = 0.2f;  // Maximální čas mezi probliknutím
    public float darkDurationBeforeTrain = 1f;

    [Header("Train Sequence")]
    public PlayableDirector trainTimeline; // Odkaz na Timeline s animací vlaku

    [Header("Audio")]
    public AudioSource levelAudioSource;
    public AudioClip warningSound;

    private void Start()
    {
        currentTimer = timeBetweenTrains;
        
        // Ujistíme se, že na začátku hry všechna světla normálně svítí
        SetLightsState(true);
    }

    private void Update()
    {
        // Timer běží pouze pokud je minihra aktivní a zrovna neprobíhá útok vlaku
        if (isMinigameActive && !isSequenceRunning)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0f)
            {
                StartCoroutine(ExecuteTrainSequence());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Zapnutí minihry při průchodu (lze dát na neviditelný BoxCollider na začátek chodby)
        if (other.CompareTag("Player") && !isMinigameActive)
        {
            isMinigameActive = true;
        }
    }

    private IEnumerator ExecuteTrainSequence()
    {
        isSequenceRunning = true;

        // 1. Varování - Zvuk
        if (levelAudioSource != null && warningSound != null)
        {
            levelAudioSource.PlayOneShot(warningSound);
        }

        // 2. Realistické blikání / praskání
        float flickerTimer = 0f;
        while (flickerTimer < flickerDuration)
        {
            foreach (Light l in targetLights)
            {
                // Každé světlo si "hodí mincí", jestli bude v tento moment svítit nebo zkratuje
                if (l != null) l.enabled = Random.value > 0.5f;
            }
            
            // Náhodná pauza vytváří ten efekt nepravidelného praskání
            float randomDelay = Random.Range(minFlickerDelay, maxFlickerDelay);
            yield return new WaitForSeconds(randomDelay);
            flickerTimer += randomDelay;
        }

        // 3. Absolutní tma před příjezdem
        SetLightsState(false);

        yield return new WaitForSeconds(darkDurationBeforeTrain);

        // 4. Spuštění vlaku (Timeline cutscéna)
        if (trainTimeline != null)
        {
            trainTimeline.Play();
            // Automaticky počká, než Timeline skončí (podle její délky)
            yield return new WaitForSeconds((float)trainTimeline.duration);
        }

        // 5. Rozsvícení a reset do další smyčky
        SetLightsState(true);

        currentTimer = timeBetweenTrains;
        isSequenceRunning = false;
    }

    // Pomocná metoda pro rychlé hromadné zapnutí/vypnutí světel
    private void SetLightsState(bool state)
    {
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = state;
        }
    }
}