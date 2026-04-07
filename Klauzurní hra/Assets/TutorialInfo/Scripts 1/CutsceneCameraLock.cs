using UnityEngine;
using UnityEngine.Playables;

public class CutsceneCameraLock : MonoBehaviour
{
    public PlayableDirector director;
    
    [Tooltip("Sem přetáhni skript, kterým se hráč normálně rozhlíží (např. MouseLook)")]
    public MonoBehaviour normalLookScript; 
    
    [Tooltip("Sem přetáhni ten vypnutý ElasticCutsceneLook z kamery")]
    public ElasticCutsceneLook elasticLook;

    void OnEnable()
    {
        if (director)
        {
            director.played += CutsceneStart;
            director.stopped += CutsceneEnd;
        }
    }

    void OnDisable()
    {
        if (director)
        {
            director.played -= CutsceneStart;
            director.stopped -= CutsceneEnd;
        }
    }

    void CutsceneStart(PlayableDirector pd)
    {
        // Vypne normální rozhlížení, zapne gumičku
        if (normalLookScript) normalLookScript.enabled = false;
        if (elasticLook) elasticLook.enabled = true;
    }

    void CutsceneEnd(PlayableDirector pd)
    {
        // Vypne gumičku, zapne zpět normální rozhlížení
        if (elasticLook) elasticLook.enabled = false;
        if (normalLookScript) normalLookScript.enabled = true;
    }
}