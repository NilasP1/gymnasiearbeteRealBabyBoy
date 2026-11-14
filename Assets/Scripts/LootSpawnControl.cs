using System.Collections.Generic;
using UnityEngine;

public class LootSpawnControl : MonoBehaviour
{
    //a list for spwnpoints in each zone to have different rarities in different zones
    [SerializeField] private List<GameObject> ForestRegionSpawnPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> MountainRegionSpawnPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> FarmlandsRegionSpawnPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> CityRegionSpawnPoints = new List<GameObject>();
    [SerializeField] private List<GameObject> MilitaryRegionSpawnPoints = new List<GameObject>();

    [SerializeField] private List<GameObject> LootPrefabs = new List<GameObject>();

    [SerializeField] private LootableObjectQuality ForestRegionQuality = LootableObjectQuality.Normal;
    [SerializeField] private LootableObjectQuality MountainRegionQuality = LootableObjectQuality.Improved;
    [SerializeField] private LootableObjectQuality FarmlandRegionQuality = LootableObjectQuality.Reinforced;
    [SerializeField] private LootableObjectQuality CityRegionQuality = LootableObjectQuality.Armored;
    [SerializeField] private LootableObjectQuality MilitaryRegionQuality = LootableObjectQuality.MilitaryGrade;


    void Start()
    {
        spawnLoot(ForestRegionSpawnPoints, ForestRegionQuality);
        spawnLoot(MountainRegionSpawnPoints, MountainRegionQuality);
        spawnLoot(FarmlandsRegionSpawnPoints, FarmlandRegionQuality);
        spawnLoot(CityRegionSpawnPoints, CityRegionQuality);
        spawnLoot(MilitaryRegionSpawnPoints, MilitaryRegionQuality);
    }

    public void spawnLoot(List<GameObject> spawnpoints, LootableObjectQuality lootquality)
    {
        // Loop through all region spawnpoints,
        // spawn a random loot prefab at each spawnpoint,
        // and assign the ForestRegionQuality to that object.
        for (int i = 0; i < spawnpoints.Count; ++i)
        {
            GameObject spawnPoint = spawnpoints[i];

            // Select a random loot prefab
            int randomLootIndex = Random.Range(0, LootPrefabs.Count);

            // Spawn the loot object
            GameObject spawnedLoot = Instantiate(
                LootPrefabs[randomLootIndex],
                spawnPoint.transform.position,
                spawnPoint.transform.rotation
            );

            // Get the LootableObject component
            LootableObject lootableObject = spawnedLoot.GetComponent<LootableObject>();

            // Assign the region's quality and initialize its stats
            if (lootableObject != null)
            {
                lootableObject.Initialize(
                    lootableObject.lootableObjectType,
                    lootquality
                );
            }
        }
    }
}
