using UnityEngine;
using UnityEngine.Playables;

public class CutsceneFixer1 : MonoBehaviour
{
    public PlayableDirector director;
    public CursorController myMouseScript; // Sem přetáhneš ten upravený script nahoře

    void OnEnable()
    {
        if (director)
        {
            director.played += ZapniCutscenu;
            director.stopped += VypniCutscenu;
        }
    }

    void OnDisable()
    {
        if (director)
        {
            director.played -= ZapniCutscenu;
            director.stopped -= VypniCutscenu;
        }
    }

    void ZapniCutscenu(PlayableDirector pd)
    {
        if (myMouseScript) myMouseScript.inCutscene = true;
    }

    void VypniCutscenu(PlayableDirector pd)
    {
        if (myMouseScript) myMouseScript.inCutscene = false;
    }
}