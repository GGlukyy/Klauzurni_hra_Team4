using UnityEngine;

// Tento øádek zajistí, že se AudioSource pøidá automaticky
[RequireComponent(typeof(AudioSource))]
public class AudioTrigger : MonoBehaviour
{
    [Header("Audio Setup")]
    [Tooltip("Pøetáhni sem pøímo zvukový soubor (.mp3, .wav)")]
    public AudioClip clipToPlay;
    [Tooltip("Vteøina, od které má zvuk zaèít hrát")]
    public float startPlaybackTime = 0f;

    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public bool stopOnExit = true;

    private AudioSource audioSource;

    private void Start()
    {
        // Najdeme AudioSource na tomto objektu a nastavíme ho
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clipToPlay;
        audioSource.playOnAwake = false; // Pojistka, aby nehrál hned po startu
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (!audioSource.isPlaying && clipToPlay != null)
            {
                audioSource.time = startPlaybackTime;
                audioSource.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (stopOnExit && other.CompareTag(playerTag))
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}