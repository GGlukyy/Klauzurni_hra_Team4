using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    private TextMeshProUGUI textElement;
    public float typingSpeed = 0.05f;

    void Awake() 
    {
        textElement = GetComponent<TextMeshProUGUI>();
    }

    // Tuhle funkci můžeš volat z Timeline přes "Signal" nebo z jiného skriptu
    public void PlayDialogue(string sentence)
    {
        StopAllCoroutines();
        StartCoroutine(TypeLog(sentence));
    }

    IEnumerator TypeLog(string fullText)
    {
        textElement.text = "";
        foreach (char c in fullText.ToCharArray())
        {
            textElement.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void ClearText()
    {
        textElement.text = "";
    }
}