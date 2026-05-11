using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public struct DialogueLine3D
{
    [Tooltip("3D TextMeshPro prvek nad hlavou NPC")]
    public TextMeshPro speaker3DText;
    [TextArea] public string sentence;
    public Color textColor;
    public float delayAfter;
}

[RequireComponent(typeof(Collider))]
public class TriggerDialogue3D : MonoBehaviour
{
    [Header("Mode")]
    public bool isConversation = false;

    [Header("Single Dialogue")]
    public TextMeshPro single3DText;
    [TextArea] public string singleSentence;
    public float clearDelay = 2f;

    [Header("Conversation Settings")]
    public DialogueLine3D[] conversationLines;

    [Header("Trigger Behavior")]
    public float typingSpeed = 0.05f;
    public bool playOnlyOnce = true;
    public bool loopContinuously = false;

    private bool hasTriggered = false;
    private bool isTyping = false;

    private void Start()
    {
        ClearAllText();

        if (loopContinuously)
        {
            ToggleVisibility(false);
            StartCoroutine(PlayDialogue(true));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (loopContinuously)
        {
            ToggleVisibility(true);
            return;
        }

        if (playOnlyOnce && hasTriggered) return;
        if (isTyping) return;

        hasTriggered = true;
        ToggleVisibility(true);
        StartCoroutine(PlayDialogue(false));
    }

    private void OnTriggerExit(Collider other)
    {
        if (loopContinuously && other.CompareTag("Player"))
        {
            ToggleVisibility(false);
        }
    }

    private IEnumerator PlayDialogue(bool loop)
    {
        isTyping = true;

        do
        {
            if (!isConversation)
            {
                yield return StartCoroutine(TypeText(single3DText, singleSentence, Color.white));
                yield return new WaitForSeconds(clearDelay);
                if (single3DText != null) single3DText.text = "";
            }
            else
            {
                foreach (var line in conversationLines)
                {
                    yield return StartCoroutine(TypeText(line.speaker3DText, line.sentence, line.textColor));
                    yield return new WaitForSeconds(line.delayAfter);
                    if (line.speaker3DText != null) line.speaker3DText.text = "";
                }
            }
        } while (loop);

        isTyping = false;
        
        if (playOnlyOnce && !loop)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator TypeText(TextMeshPro tmp, string text, Color color)
    {
        if (tmp == null) yield break;
        
        tmp.text = "";
        tmp.color = color;
        
        foreach (char c in text.ToCharArray())
        {
            tmp.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void ToggleVisibility(bool state)
    {
        if (!isConversation && single3DText != null)
        {
            single3DText.enabled = state;
        }
        else
        {
            foreach (var line in conversationLines)
            {
                if (line.speaker3DText != null) line.speaker3DText.enabled = state;
            }
        }
    }

    private void ClearAllText()
    {
        if (single3DText != null) single3DText.text = "";
        foreach (var line in conversationLines)
        {
            if (line.speaker3DText != null) line.speaker3DText.text = "";
        }
    }
}