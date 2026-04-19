using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DelayedEvent : MonoBehaviour
{
    [Header("Časovač")]
    public float delayInSeconds = 3f;

    [Header("Co se stane po uplynutí času")]
    public UnityEvent onTimerComplete;

    // Tuto metodu zavoláš přes jiný event (např. z PickupItem)
    public void StartTimer()
    {
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        yield return new WaitForSeconds(delayInSeconds);
        onTimerComplete?.Invoke();
    }
}