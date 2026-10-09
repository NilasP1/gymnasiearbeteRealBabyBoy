using UnityEngine;

public class CarUpgrading : MonoBehaviour
{
    [Header("Car Part Prefabs")]
    public LootableObject TiresPrefab;
    public LootableObject EnginePrefab;
    public LootableObject BodyPrefab;
    public LootableObject LightsPrefab;
    public LootableObject BrakesPrefab;
    public LootableObject SuspensionPrefab;

    [Header("Equipped Car Parts")]
    public LootableObject EquipedCarTires;
    public LootableObject EquipedCarEngine;
    public LootableObject EquipedCarBody;
    public LootableObject EquipedCarLights;
    public LootableObject EquipedCarBrakes;
    public LootableObject EquipedCarSuspension;

    [Header("Unequipped Part")]
    public LootableObject DeEquipedItem;

    [Header("Car Controller")]
    public CarController carController;

    private void Awake()
    {
        // Create the starting parts with Normal quality.
        EquipedCarTires = CreateStartingPart(
            TiresPrefab, LootableObjectType.Tires);

        EquipedCarEngine = CreateStartingPart(
            EnginePrefab, LootableObjectType.Engine);

        EquipedCarBody = CreateStartingPart(
            BodyPrefab, LootableObjectType.Body);

        EquipedCarLights = CreateStartingPart(
            LightsPrefab, LootableObjectType.Light);

        EquipedCarBrakes = CreateStartingPart(
            BrakesPrefab, LootableObjectType.Brakes);

        EquipedCarSuspension = CreateStartingPart(
            SuspensionPrefab, LootableObjectType.Suspension);

        UpdateCarStats();
    }

    private LootableObject CreateStartingPart(
        LootableObject prefab,
        LootableObjectType type)
    {
        if (prefab == null)
        {
            Debug.LogError("Missing car part prefab for: " + type);
            return null;
        }

        LootableObject part = Instantiate(prefab);

        part.Initialize(type, LootableObjectQuality.Normal);

        // These objects store stats only, so their models stay hidden.
        part.gameObject.SetActive(false);

        return part;
    }

    public void EquipCarPart(LootableObject newPart)
    {
        if (newPart == null)
        {
            Debug.LogWarning("Cannot equip a null car part.");
            return;
        }

        DeEquipedItem = null;

        switch (newPart.lootableObjectType)
        {
            case LootableObjectType.Tires:
                DeEquipedItem = EquipedCarTires;
                EquipedCarTires = newPart;
                break;

            case LootableObjectType.Engine:
                DeEquipedItem = EquipedCarEngine;
                EquipedCarEngine = newPart;
                break;

            case LootableObjectType.Body:
                DeEquipedItem = EquipedCarBody;
                EquipedCarBody = newPart;
                break;

            case LootableObjectType.Light:
                DeEquipedItem = EquipedCarLights;
                EquipedCarLights = newPart;
                break;

            case LootableObjectType.Brakes:
                DeEquipedItem = EquipedCarBrakes;
                EquipedCarBrakes = newPart;
                break;

            case LootableObjectType.Suspension:
                DeEquipedItem = EquipedCarSuspension;
                EquipedCarSuspension = newPart;
                break;

            default:
                Debug.LogWarning(
                    "Unknown car part type: " + newPart.lootableObjectType);
                return;
        }

        // Equipped parts are stat data only; don't display their models.
        newPart.gameObject.SetActive(false);

        // Refresh the car's stats.
        UpdateCarStats();
    }

    private void UpdateCarStats()
    {
        if (carController == null)
            return;

        if (EquipedCarTires == null ||
            EquipedCarEngine == null ||
            EquipedCarBody == null ||
            EquipedCarLights == null ||
            EquipedCarBrakes == null ||
            EquipedCarSuspension == null)
        {
            return;
        }

        carController.ChangeCarStats(
            EquipedCarEngine.EngineTorque,             // motorTorque
            EquipedCarBrakes.brakeTorque,              // brakeTorque
            EquipedCarBody.BodyAirResistance,          // maxSpeed
            EquipedCarLights.LightIntensity,           // lightintensity
            EquipedCarSuspension.suspensiondistance,    // suspension
            EquipedCarTires.TireGrip                   // tiregrip
        );
    }

}