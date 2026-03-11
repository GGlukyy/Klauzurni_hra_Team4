using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Patrolling, Following, Attacking }
    public EnemyState currentState = EnemyState.Patrolling;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;
    private int currentPatrolIndex = 0;
    private bool isWaiting = false;

    [Header("Detection & Chase Settings")]
    public Transform playerTransform;
    public float detectionRange = 10f;
    public float viewAngle = 90f;
    public float timeToLosePlayer = 3f;
    private float playerLostTimer = 0f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    private bool isBiting = false;

    private NavMeshAgent agent;
    private Animator animator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (patrolPoints.Length > 0)
            GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case EnemyState.Patrolling: // [08:42] Hlídkování
                Patrol();
                if (distanceToPlayer < detectionRange && CanSeePlayer())
                {
                    currentState = EnemyState.Following;
                }
                break;

            case EnemyState.Following: // [10:34] Pronásledování
                FollowPlayer();
                if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attacking;
                    StartAttack();
                }
                else if (!CanSeePlayer())
                {
                    playerLostTimer += Time.deltaTime;
                    if (playerLostTimer >= timeToLosePlayer)
                    {
                        currentState = EnemyState.Patrolling;
                        GoToClosestPatrolPoint();
                    }
                }
                else
                {
                    playerLostTimer = 0f; // Reset časovače, pokud hráče znovu vidí
                }
                break;

            case EnemyState.Attacking: // [13:01] Útok
                Attack();
                // Návrat k pronásledování, pokud animace skončila a hráč je daleko
                if (!isBiting && distanceToPlayer > attackRange)
                {
                    agent.isStopped = false;
                    currentState = EnemyState.Following;
                }
                break;
        }

        UpdateAnimations();
    }

    // --- HLÍDKOVÁNÍ ---
    void Patrol()
    {
        if (isWaiting) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void GoToClosestPatrolPoint()
    {
        float closestDistance = Mathf.Infinity;
        int closestIndex = 0;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestIndex = i;
            }
        }
        currentPatrolIndex = closestIndex;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(patrolWaitTime);
        agent.isStopped = false;
        GoToNextPatrolPoint();
        isWaiting = false;
    }

    // --- DETEKCE ---
    bool CanSeePlayer() // [09:02] Logika pro viditelnost hráče
    {
        return IsFacingPlayer() && HasClearPathToPlayer();
    }

    bool IsFacingPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < viewAngle / 2f; // Je hráč v zorném poli?
    }

    bool HasClearPathToPlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        
        // Zkontroluje, zda není mezi nepřítelem a hráčem překážka (zeď atd.)
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distance))
        {
            if (hit.transform == playerTransform) return true;
            return false;
        }
        return true;
    }

    // --- PRONÁSLEDOVÁNÍ ---
    void FollowPlayer()
    {
        agent.SetDestination(playerTransform.position);
    }

    // --- ÚTOK ---
    void StartAttack()
    {
        agent.isStopped = true;
        isBiting = true;
        animator.SetTrigger("bite");
    }

    void Attack()
    {
        agent.isStopped = true;
        // Plynulé natáčení za hráčem i během útočení
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0; 
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }

    // Tuto funkci musíš zavolat přes Animation Event! [13:47]
    public void OnBiteAnimationEnd()
    {
        isBiting = false;
    }

    // --- ANIMACE ---
    void UpdateAnimations() // [06:24]
    {
        bool isWalking = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isWalking);
    }
}