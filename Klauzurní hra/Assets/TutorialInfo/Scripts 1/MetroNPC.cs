using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class MetroNPC : MonoBehaviour
{
    public enum State { Patrolling, FollowingPlayer, InCutscene }
    public State currentState = State.Patrolling;

    [Header("Player & Cutscene")]
    public Transform player;
    public float detectionRange = 12f;
    public float viewAngle = 90f;
    public float cutsceneTriggerRange = 2f;
    public UnityEvent OnCatchPlayer; 

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    private int currentPatrolIndex;
    private bool isWaiting;

    private float playerIgnoreTimer; 

    private NavMeshAgent agent;
    private Animator anim;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (patrolPoints.Length > 0) 
            GoToNextPatrolPoint();
    }

    void Update()
    {
        if (playerIgnoreTimer > 0) playerIgnoreTimer -= Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (playerIgnoreTimer <= 0) CheckForPlayer(distanceToPlayer);
                break;

            case State.FollowingPlayer:
                agent.SetDestination(player.position);
                
                if (distanceToPlayer <= cutsceneTriggerRange)
                {
                    TriggerCutscene();
                }
                else if (!CanSeePlayer(distanceToPlayer))
                {
                    currentState = State.Patrolling;
                    GoToClosestPatrolPoint();
                }
                break;

            case State.InCutscene:
                // Nic nedělá, čeká na konec cutscény
                break;
        }

        UpdateAnimator();
    }

    // --- ANIMACE ---
    void UpdateAnimator()
    {
        // Kontroluje, jestli má agent zadanou cestu a není úplně v cíli
        bool isMoving = agent.hasPath && agent.remainingDistance > 0.1f && !agent.isStopped;
        anim.SetBool("isWalking", isMoving);
    }

    // --- HLÍDKOVÁNÍ ---
    void Patrol()
    {
        if (isWaiting || currentState != State.Patrolling) return;
        
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void GoToClosestPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        float closest = Mathf.Infinity;
        int index = 0;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float d = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (d < closest) { closest = d; index = i; }
        }
        currentPatrolIndex = index;
        GoToNextPatrolPoint();
    }

    IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(patrolWaitTime);
        if (currentState == State.Patrolling) GoToNextPatrolPoint();
        isWaiting = false;
    }

    // --- DETEKCE HRÁČE ---
    void CheckForPlayer(float dist)
    {
        if (dist < detectionRange && CanSeePlayer(dist))
        {
            currentState = State.FollowingPlayer;
            agent.isStopped = false;
        }
    }

    bool CanSeePlayer(float dist)
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, dist))
            {
                if (hit.transform == player) return true;
            }
        }
        return false;
    }

    // --- CUTSCÉNA ---
    void TriggerCutscene()
    {
        currentState = State.InCutscene;
        
        // Natvrdo zastavit agenta
        agent.isStopped = true;
        agent.velocity = Vector3.zero; 
        agent.ResetPath(); 
        
        OnCatchPlayer.Invoke(); 
    }

    // Tuto funkci zavolej z tvojí cutscény (přes UnityEvent, Timeline Signal atd.), až skončí
    public void ResumeAfterCutscene()
    {
        currentState = State.Patrolling;
        playerIgnoreTimer = 10f; // Bude hráče 10 sekund ignorovat, aby mohl odejít
        agent.isStopped = false;
        GoToClosestPatrolPoint();
    }
}