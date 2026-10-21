using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class walkerAI : MonoBehaviour
{
    public Transform player;
    public CheckPlayerAgro checkPlayerAgro;
    public float roamingRange = 10f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (checkPlayerAgro == null)
            checkPlayerAgro = GetComponentInChildren<CheckPlayerAgro>();
    }

    void Update()
    {
        if (player == null || checkPlayerAgro == null)
            return;

        if (checkPlayerAgro.PlayerInAgroRange && CanSeePlayer())
        {
            agent.SetDestination(player.position);
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * roamingRange;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, roamingRange, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dir = player.position - transform.position;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position, dir.normalized, dir.magnitude,
            ~0, QueryTriggerInteraction.Ignore);

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // Skip the zombie itself
            if (hit.collider.transform.IsChildOf(transform))
                continue;

            // First thing that isn't the zombie must be the player
            return hit.collider.transform.root.CompareTag("Player")
                || hit.collider.CompareTag("Player");
        }

        return false;
    }
}