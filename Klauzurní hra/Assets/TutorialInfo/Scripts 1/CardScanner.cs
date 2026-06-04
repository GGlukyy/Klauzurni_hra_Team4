using UnityEngine;
using UnityEngine.Playables;

public class CardScanner : MonoBehaviour
{
    [Header("Nastavení Terminálu")]
    [Tooltip("Vyžaduje tento terminál vůbec nějakou kartu? Pokud ne, otevře se rovnou.")]
    public bool requiresCard = true;

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

        // Propustí hráče buď pokud karta není potřeba, nebo má tu správnou
        if (!requiresCard || itemNameInHand == requiredCardName)
        {
            if (!requiresCard)
                Debug.Log("Karta není vyžadována. Spouští se otevření dveří.");
            else
                Debug.Log("Karta přijata! Spouští se otevření dveří.");

            isAlreadyScanned = true;

            if (doorCutscene != null)
            {
                doorCutscene.Play();
            }

            return true; // Vrátí true -> úspěch
        }
        else
        {
            Debug.Log("Přístup odepřen: Nemáš správnou kartu.");
            return false; // Vrátí false -> špatná karta nebo prázdné ruce
        }
    }
}