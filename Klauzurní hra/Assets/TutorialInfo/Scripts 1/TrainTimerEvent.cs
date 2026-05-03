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
    public float flickerSpeed = 0.1f;
    public float darkDurationBeforeTrain = 1f;

    [Header("Train Sequence")]
    public PlayableDirector trainTimeline; // Odkaz na Timeline s animací vlaku

    [Header("Audio")]
    public AudioSource levelAudioSource;
    public AudioClip warningSound;

    private void Start()
    {
        currentTimer = timeBetweenTrains;
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

        // 1. Varování - Zvuk a blikání
        if (levelAudioSource != null && warningSound != null)
        {
            levelAudioSource.PlayOneShot(warningSound);
        }

        float flickerTimer = 0f;
        while (flickerTimer < flickerDuration)
        {
            foreach (Light l in targetLights)
            {
                if (l != null) l.enabled = !l.enabled;
            }
            yield return new WaitForSeconds(flickerSpeed);
            flickerTimer += flickerSpeed;
        }

        // 2. Absolutní tma před příjezdem
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = false;
        }

        yield return new WaitForSeconds(darkDurationBeforeTrain);

        // 3. Spuštění vlaku (Timeline cutscéna)
        if (trainTimeline != null)
        {
            trainTimeline.Play();
            // Automaticky počká, než Timeline skončí (podle její délky)
            yield return new WaitForSeconds((float)trainTimeline.duration);
        }

        // 4. Rozsvícení a reset do další smyčky
        foreach (Light l in targetLights)
        {
            if (l != null) l.enabled = true;
        }

        currentTimer = timeBetweenTrains;
        isSequenceRunning = false;
    }
}