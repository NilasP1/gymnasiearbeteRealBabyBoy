
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
    public LootableObject EquippedCarTires;
    public LootableObject EquippedCarEngine;
    public LootableObject EquippedCarBody;
    public LootableObject EquippedCarLights;
    public LootableObject EquippedCarBrakes;
    public LootableObject EquippedCarSuspension;

    [Header("Unequipped Part")]
    public LootableObject DeEquipedItem;

    [Header("Car Controller")]
    public CarController carController;

    private void Awake()
    {
        EquippedCarTires = CreateStartingPart(
            TiresPrefab, LootableObjectType.Tires);

        EquippedCarEngine = CreateStartingPart(
            EnginePrefab, LootableObjectType.Engine);

        EquippedCarBody = CreateStartingPart(
            BodyPrefab, LootableObjectType.Body);

        EquippedCarLights = CreateStartingPart(
            LightsPrefab, LootableObjectType.Light);

        EquippedCarBrakes = CreateStartingPart(
            BrakesPrefab, LootableObjectType.Brakes);

        EquippedCarSuspension = CreateStartingPart(
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
        part.gameObject.SetActive(false);

        return part;
    }

    public LootableObject EquipCarPart(LootableObject newPart)
    {
        if (!TryEquipCarPart(newPart, out LootableObject oldPart))
            return null;

        return oldPart;
    }

    public bool TryEquipCarPart(
        LootableObject newPart,
        out LootableObject oldPart)
    {
        oldPart = null;

        if (newPart == null)
            return false;

        switch (newPart.lootableObjectType)
        {
            case LootableObjectType.Tires:
                oldPart = EquippedCarTires;
                EquippedCarTires = newPart;
                break;

            case LootableObjectType.Engine:
                oldPart = EquippedCarEngine;
                EquippedCarEngine = newPart;
                break;

            case LootableObjectType.Body:
                oldPart = EquippedCarBody;
                EquippedCarBody = newPart;
                break;

            case LootableObjectType.Light:
                oldPart = EquippedCarLights;
                EquippedCarLights = newPart;
                break;

            case LootableObjectType.Brakes:
                oldPart = EquippedCarBrakes;
                EquippedCarBrakes = newPart;
                break;

            case LootableObjectType.Suspension:
                oldPart = EquippedCarSuspension;
                EquippedCarSuspension = newPart;
                break;

            default:
                return false;
        }

        DeEquipedItem = oldPart;

        newPart.gameObject.SetActive(false);

        UpdateCarStats();

        return true;
    }

    public LootableObject TakeCarPart(LootableObjectType type)
    {
        LootableObject part = null;

        switch (type)
        {
            case LootableObjectType.Tires:
                part = EquippedCarTires;
                EquippedCarTires = null;
                break;

            case LootableObjectType.Engine:
                part = EquippedCarEngine;
                EquippedCarEngine = null;
                break;

            case LootableObjectType.Body:
                part = EquippedCarBody;
                EquippedCarBody = null;
                break;

            case LootableObjectType.Light:
                part = EquippedCarLights;
                EquippedCarLights = null;
                break;

            case LootableObjectType.Brakes:
                part = EquippedCarBrakes;
                EquippedCarBrakes = null;
                break;

            case LootableObjectType.Suspension:
                part = EquippedCarSuspension;
                EquippedCarSuspension = null;
                break;
        }

        DeEquipedItem = part;

        UpdateCarStats();

        return part;
    }

    private void UpdateCarStats()
    {
        if (carController == null)
            return;

        if (EquippedCarTires == null ||
            EquippedCarEngine == null ||
            EquippedCarBody == null ||
            EquippedCarLights == null ||
            EquippedCarBrakes == null ||
            EquippedCarSuspension == null)
            return;

        carController.ChangeCarStats(
            EquippedCarEngine.EngineTorque,
            EquippedCarBrakes.brakeTorque,
            EquippedCarBody.BodyAirResistance,
            EquippedCarLights.LightIntensity,
            EquippedCarSuspension.suspensiondistance,
            EquippedCarTires.TireGrip
        );
    }
}
