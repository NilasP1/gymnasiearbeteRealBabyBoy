using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieManager : MonoBehaviour
{
    public float circleRadius = 10f;

    public Transform player;

    public LayerMask groundLayer;
    public float groundRayHeight = 50f;
    public float groundRayDistance = 100f;
    public float SpawnHeightAboveGround = 2;

    public List<GameObject> SpawnPoints = new List<GameObject>();
    public List<GameObject> Zombies = new List<GameObject>();

    public int CurrentzombieCount = 0;
    public int MaxZombieCount = 15;
    public float ZombieDespawnRadius = 30f;
    public GameObject zombiePrefab;


    void Update()
    {
        UpdateSpawnPoints();

        if (CurrentzombieCount < MaxZombieCount)
        {
            SpawnZombie();
        }

        foreach (GameObject zombie in Zombies.ToArray())
        {
            if (zombie == null)
            {
                Zombies.Remove(zombie);
                CurrentzombieCount--;
                continue;
            }
            float distanceToPlayer = Vector3.Distance(zombie.transform.position, player.position);
            if (distanceToPlayer > ZombieDespawnRadius)
            {
                Destroy(zombie);
                Zombies.Remove(zombie);
                CurrentzombieCount--;
            }
        }
    }

    void UpdateSpawnPoints()
    {
        if (player == null || SpawnPoints.Count == 0)
            return;

        int count = SpawnPoints.Count;

        for (int i = 0; i < count; i++)
        {
            GameObject spawnPoint = SpawnPoints[i];

            if (spawnPoint == null)
                continue;

            // Evenly space the spawn points around the circle
            float angle = (360f / count) * i;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(radians),
                0f,
                Mathf.Sin(radians)
            );

            // Calculate position on the circle
            Vector3 position = player.position + direction * circleRadius;

            // Start above the terrain and raycast downward
            Vector3 rayOrigin = new Vector3(
                position.x,
                player.position.y + groundRayHeight,
                position.z
            );

            if (Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                groundRayDistance,
                groundLayer))
            {
                // Use the ground height
                position.y = hit.point.y;
            }

            spawnPoint.transform.position = position + Vector3.up * SpawnHeightAboveGround;
        }
    }

    public void SpawnZombie()
    {
        if (SpawnPoints.Count == 0)
            return;

        int randomIndex = Random.Range(0, SpawnPoints.Count);
        GameObject spawnPoint = SpawnPoints[randomIndex];

        if (spawnPoint == null)
            return;

        if (zombiePrefab != null)
        {
            GameObject newZombie = Instantiate(
                zombiePrefab,
                spawnPoint.transform.position,
                Quaternion.identity
            );

            // Ensure NavMeshAgent is on the NavMesh (try to snap / warp it)
            NavMeshAgent agent = newZombie.GetComponent<NavMeshAgent>();
            if (agent != null && !agent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(newZombie.transform.position, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
                {
                    agent.Warp(navHit.position);
                }
                else if (NavMesh.SamplePosition(newZombie.transform.position + Vector3.down * 1f, out navHit, 5f, NavMesh.AllAreas))
                {
                    agent.Warp(navHit.position);
                }
            }

            // Assign walkerAI references if present
            var walker = newZombie.GetComponent<walkerAI>();
            if (walker != null)
            {
                if (walker.player == null)
                    walker.player = player;

                if (walker.checkPlayerAgro == null)
                    walker.checkPlayerAgro = newZombie.GetComponentInChildren<CheckPlayerAgro>();
            }

            Zombies.Add(newZombie);
            CurrentzombieCount++;
        }
    }

    public class walkerAI : MonoBehaviour
    {
        public Transform player;
        public CheckPlayerAgro checkPlayerAgro;

        public float roamingRange = 10f;
        public float eyeHeight = 1.5f;
        public float targetHeight = 1.0f;

        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            // Try to auto-assign player if not set in inspector
            if (player == null)
            {
                var found = GameObject.FindGameObjectWithTag("Player");
                if (found != null)
                    player = found.transform;
                else
                    Debug.LogWarning($"{name}: walkerAI could not find GameObject with tag 'Player'. Assign the player reference in the inspector.");
            }

            // Try to auto-assign CheckPlayerAgro (commonly on a child trigger)
            if (checkPlayerAgro == null)
            {
                checkPlayerAgro = GetComponentInChildren<CheckPlayerAgro>();
                if (checkPlayerAgro == null)
                    Debug.LogWarning($"{name}: walkerAI could not find a CheckPlayerAgro component in children. Ensure an agro trigger exists and CheckPlayerAgro is attached.");
            }

            if (agent == null)
                Debug.LogWarning($"{name}: NavMeshAgent missing. Add a NavMeshAgent to the zombie prefab.");
        }

        void Update()
        {
            if (player == null || checkPlayerAgro == null || agent == null || !agent.isOnNavMesh)
                return;

            if (checkPlayerAgro.PlayerInAgroRange && CanSeePlayer())
            {
                // Chase the player
                agent.SetDestination(player.position);
            }
            else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                // Roam: pick a new point when we are done
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * roamingRange;

                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, roamingRange, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);
            }
        }

        bool CanSeePlayer()
        {
            Vector3 origin = transform.position + Vector3.up * eyeHeight;
            Vector3 target = player.position + Vector3.up * targetHeight;
            Vector3 dir = target - origin;

            RaycastHit[] hits = Physics.RaycastAll(origin, dir.normalized, dir.magnitude);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.transform.IsChildOf(transform))
                    continue;

                return hit.collider.CompareTag("Player")
                    || hit.collider.transform.root.CompareTag("Player");
            }

            return false;
        }
    }
}