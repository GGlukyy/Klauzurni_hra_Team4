using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource), typeof(Animator))]
public class NPCFriend : MonoBehaviour
{
    private NavMeshAgent agent;
    private AudioSource audioSource;
    [HideInInspector] public Animator anim;

    [Header("Otravné hlášky při útěku")]
    public AudioClip[] hurryClips;

    private Coroutine shoutingCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    public void MoveToPoint(Transform destination, float speed = 4f)
    {
        agent.speed = speed;
        agent.SetDestination(destination.position);
        anim.SetBool("IsRunning", true); // Spustí animaci běhu
    }

    public bool HasReachedDestination()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                anim.SetBool("IsRunning", false); // Zastaví běh
                return true;
            }
        }
        return false;
    }

    public void Say(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    // Zapne náhodné pokřikování každé 3-5 sekund
    public void StartAnnoyingShouts()
    {
        if (shoutingCoroutine == null && hurryClips.Length > 0)
            shoutingCoroutine = StartCoroutine(ShoutLoop());
    }

    public void StopAnnoyingShouts()
    {
        if (shoutingCoroutine != null)
        {
            StopCoroutine(shoutingCoroutine);
            shoutingCoroutine = null;
        }
    }

    private IEnumerator ShoutLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 5f));
            AudioClip randomClip = hurryClips[Random.Range(0, hurryClips.Length)];
            Say(randomClip);
        }
    }
}