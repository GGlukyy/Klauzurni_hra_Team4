using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Stealth & Detection")]
    public Transform player;
    public float viewDistance = 10f; // Jak daleko dohlédne
    [Range(0, 360)] public float viewAngle = 90f; // Zorný úhel
    public LayerMask obstacleMask; // Vrstva překážek (zdi atd.), aby neviděl přes zdi

    private NavMeshAgent agent;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // Pokud není hráč přiřazen, zkusí ho najít podle tagu
        if (player == null) 
            player = GameObject.FindGameObjectWithTag("Player").transform;

        GotoNextPoint();
    }

    void Update()
    {
        CheckLineOfSight();

        if (isChasing)
        {
            // Jde po hráči
            agent.SetDestination(player.position);
        }
        else
        {
            // Klasická patrola
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GotoNextPoint();
            }
        }
    }

    void CheckLineOfSight()
    {
        if (player == null) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Je hráč dostatečně blízko?
        // 2. Je hráč v zorném úhlu? (dělíme 2, protože úhel se počítá od středu na obě strany)
        if (distanceToPlayer < viewDistance && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2f)
        {
            // 3. Brání něco ve výhledu? (Raycast míří lehce nad zem, např. z výšky očí)
            Vector3 rayStart = transform.position + Vector3.up * 1.5f; 
            if (!Physics.Raycast(rayStart, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                isChasing = true; // Vidí hráče!
                return;
            }
        }
        
        // Hráč utekl z dohledu (tady se dá později přidat stav "Hledání")
        if (isChasing) 
        {
            isChasing = false;
            GotoNextPoint(); // Vrátí se na patrolu
        }
    }

    void GotoNextPoint()
    {
        if (waypoints.Length == 0) return;
        agent.destination = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    // Pomůcka pro Editor: Vykreslí zorný dosah, když na AI klikneš
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}