using UnityEngine;
using UnityEngine.Playables;

public class CardScanner : MonoBehaviour
{
    [Header("Nastavení Terminálu")]
    [Tooltip("Přesný název předmětu (karty), který otevírá tyto dveře")]
    public string requiredCardName = "Level1_Keycard";

    [Tooltip("Zaškrtni, pokud má terminál kartu po úspěšném použití zničit/sežrat")]
    public bool consumeCard = false;

    [Header("Cutscéna (Timeline)")]
    [Tooltip("PlayableDirector s tvou Timeline animací otevírání")]
    public PlayableDirector doorCutscene;

    private bool isAlreadyScanned = false;

    public bool TryScanCard(string itemNameInHand)
    {
        if (isAlreadyScanned) return false;

        if (itemNameInHand == requiredCardName)
        {
            Debug.Log("Karta přijata! Spouští se otevření dveří.");
            isAlreadyScanned = true;

            if (doorCutscene != null)
            {
                doorCutscene.Play();
            }

            return true; // Vrátí true -> karta byla v pořádku
        }
        else
        {
            Debug.Log("Přístup odepřen: Nemáš správnou kartu.");
            return false; // Vrátí false -> špatná karta
        }
    }
}