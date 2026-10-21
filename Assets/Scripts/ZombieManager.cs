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

}