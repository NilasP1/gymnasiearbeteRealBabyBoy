
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
    public float navMeshSampleDistance = 2f;
    public int currentSpawnPointIndex = 0;

    public int spawnPointCount = 8;
    private List<Transform> SpawnPoints = new List<Transform>();

    public List<GameObject> Zombies = new List<GameObject>();

    public int CurrentzombieCount = 0;
    public int MaxZombieCount = 15;
    public float ZombieDespawnRadius = 30f;
    public GameObject zombiePrefab;

    void Start()
    {
        CreateSpawnPoints();
    }

    void Update()
    {
        UpdateSpawnPoints();

        // Remove destroyed zombies and synchronize the count
        Zombies.RemoveAll(zombie => zombie == null);
        CurrentzombieCount = Zombies.Count;

        if (CurrentzombieCount < MaxZombieCount)
        {
            SpawnZombie();
        }

        foreach (GameObject zombie in Zombies.ToArray())
        {
            if (zombie == null || player == null)
                continue;

            float distanceToPlayer = Vector3.Distance(
                zombie.transform.position,
                player.position
            );

            if (distanceToPlayer > ZombieDespawnRadius)
            {
                Zombies.Remove(zombie);
                CurrentzombieCount = Zombies.Count;
                Destroy(zombie);
            }
        }
    }

    void CreateSpawnPoints()
    {
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("ZombieSpawnPoint"))
                Destroy(child.gameObject);
        }

        SpawnPoints.Clear();

        for (int i = 0; i < spawnPointCount; i++)
        {
            GameObject spawnPoint = new GameObject(
                "ZombieSpawnPoint_" + i
            );

            spawnPoint.transform.SetParent(transform);
            SpawnPoints.Add(spawnPoint.transform);
        }
    }

    void UpdateSpawnPoints()
    {
        if (player == null || SpawnPoints.Count == 0)
            return;

        int count = SpawnPoints.Count;

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = SpawnPoints[i];

            float angle = (360f / count) * i;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(radians),
                0f,
                Mathf.Sin(radians)
            );

            Vector3 rayOrigin = player.position + Vector3.up * 1f;

            Vector3 position;

            if (Physics.Raycast(
                rayOrigin,
                direction,
                out RaycastHit obstacleHit,
                circleRadius,
                groundLayer))
            {
                position = obstacleHit.point - direction * 1f;
            }
            else
            {
                position = player.position + direction * circleRadius;
            }

            Vector3 groundRayOrigin =
                position + Vector3.up * groundRayHeight;

            if (Physics.Raycast(
                groundRayOrigin,
                Vector3.down,
                out RaycastHit groundHit,
                groundRayDistance,
                groundLayer))
            {
                position.y = groundHit.point.y + SpawnHeightAboveGround;
            }

            spawnPoint.position = position;
        }
    }

    public void SpawnZombie()
    {
        if (SpawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points created!");
            return;
        }

        if (zombiePrefab == null)
        {
            Debug.LogWarning("Zombie prefab is missing!");
            return;
        }

        Transform spawnPoint = SpawnPoints[currentSpawnPointIndex];

        currentSpawnPointIndex =
            (currentSpawnPointIndex + 1) % SpawnPoints.Count;

        if (spawnPoint == null)
            return;

        if (!NavMesh.SamplePosition(
            spawnPoint.position,
            out NavMeshHit navHit,
            navMeshSampleDistance,
            NavMesh.AllAreas))
        {
            Debug.Log("No NavMesh at spawn point: " + spawnPoint.name);
            return;
        }

        GameObject newZombie = Instantiate(
            zombiePrefab,
            navHit.position,
            Quaternion.identity
        );

        var walker = newZombie.GetComponent<walkerAI>();

        if (walker != null)
        {
            walker.player = player;

            if (walker.checkPlayerAgro == null)
            {
                walker.checkPlayerAgro =
                    newZombie.GetComponentInChildren<CheckPlayerAgro>();
            }
        }

        Zombies.Add(newZombie);
        CurrentzombieCount++;

        Debug.Log("Zombie spawned at: " + spawnPoint.name);
    }
}
