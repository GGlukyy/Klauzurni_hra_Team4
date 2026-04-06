using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("Propojení")]
    public PlayableDirector director;
    public MonoBehaviour[] scriptsToDisable; // Zde přetáhni skripty pohybu a kamery

    void Start()
    {
        // 1. Vypneme ovládání hráče
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        // 2. Řekneme Unity, ať zavolá funkci OnCutsceneEnded, až Timeline skončí
        if (director != null)
        {
            director.stopped += OnCutsceneEnded;
        }
    }

    // Přidej do OnCutsceneEnded v CutsceneManager.cs
    void OnCutsceneEnded(PlayableDirector pd)
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }

        // NAJDE A VYPNEME CONSTRAINT
        var constraint = Camera.main.GetComponent<UnityEngine.Animations.LookAtConstraint>();
        if (constraint != null) constraint.enabled = false; 
    }

    void OnDestroy()
    {
        // Odhlášení z eventu (dobrá praxe, aby nedošlo k chybám v paměti)
        if (director != null) director.stopped -= OnCutsceneEnded;
    }
}