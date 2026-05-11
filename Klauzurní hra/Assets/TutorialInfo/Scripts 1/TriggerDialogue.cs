using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TriggerDialogue : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI textElement;

    [Header("Dialogue Settings")]
    [TextArea] public string sentence;
    public float typingSpeed = 0.05f;
    public float clearDelay = 2f;

    [Header("Trigger Behavior")]
    public bool playOnlyOnce = true;
    [Tooltip("Pokud je zapnuto, text se neustále píše na pozadí. Vstupem do triggeru ho jen zviditelníš.")]
    public bool loopContinuously = false;

    private bool hasTriggered = false;
    private bool isTyping = false;

    private void Start()
    {
        if (loopContinuously)
        {
            textElement.text = "";
            textElement.enabled = false; // Skryjeme na začátku, dokud hráč nevejde
            StartCoroutine(LoopTextRoutine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Pokud to jede ve smyčce na pozadí, jen zviditelníme text
        if (loopContinuously)
        {
            textElement.enabled = true;
            return;
        }

        // Klasické spuštění (jednorázové nebo opakované, jakmile se dopíše)
        if (playOnlyOnce && hasTriggered) return;
        if (isTyping) return;

        hasTriggered = true;
        textElement.enabled = true;
        StartCoroutine(TypeLogAndClear());
    }

    private void OnTriggerExit(Collider other)
    {
        // Skrytí textu při opuštění zóny (platí jen pro looping)
        if (loopContinuously && other.CompareTag("Player"))
        {
            textElement.enabled = false;
        }
    }

    // Coroutine pro jednorázové / občasné spuštění
    private IEnumerator TypeLogAndClear()
    {
        isTyping = true;
        textElement.text = "";
        
        foreach (char c in sentence.ToCharArray())
        {
            textElement.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(clearDelay);
        textElement.text = "";
        isTyping = false;

        if (playOnlyOnce)
        {
            Destroy(gameObject);
        }
    }

    // Coroutine pro nekonečnou smyčku na pozadí
    private IEnumerator LoopTextRoutine()
    {
        while (true)
        {
            textElement.text = "";
            foreach (char c in sentence.ToCharArray())
            {
                textElement.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(clearDelay);
        }
    }
}