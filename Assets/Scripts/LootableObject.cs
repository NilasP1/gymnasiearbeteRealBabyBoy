
using UnityEngine;

public class LootableObject : MonoBehaviour
{
    [Header("Item Settings")]
    public LootableObjectType lootableObjectType;
    public LootableObjectQuality lootableObjectQuality;

    [Header("Car Part Stats")]
    public float TireGrip;
    public float EngineTorque;
    public float BodyAirResistance;
    public float LightIntensity;
    public float brakeTorque;
    public float suspensiondistance;

    private void Start()
    {
        Initialize(lootableObjectType, lootableObjectQuality);
    }

    public void Initialize(
        LootableObjectType type,
        LootableObjectQuality quality)
    {
        lootableObjectType = type;
        lootableObjectQuality = quality;

        TireGrip = 0f;
        EngineTorque = 0f;
        BodyAirResistance = 0f;
        LightIntensity = 0f;
        brakeTorque = 0f;
        suspensiondistance = 0f;

        switch (type)
        {
            case LootableObjectType.Tires:
                TireGrip = GetTireGrip(quality);
                break;

            case LootableObjectType.Engine:
                EngineTorque = GetEngineTorque(quality);
                break;

            case LootableObjectType.Body:
                BodyAirResistance = GetBodyAirResistance(quality);
                break;

            case LootableObjectType.Light:
                LightIntensity = GetLightIntensity(quality);
                break;

            case LootableObjectType.Brakes:
                brakeTorque = GetBrakeTorque(quality);
                break;

            case LootableObjectType.Suspension:
                suspensiondistance = GetSuspensionDistance(quality);
                break;
        }
    }

    private float GetTireGrip(LootableObjectQuality quality)
    {
        return quality switch
        {
            LootableObjectQuality.Normal => 1.0f,
            LootableObjectQuality.Improved => 1.2f,
            LootableObjectQuality.Reinforced => 1.5f,
            LootableObjectQuality.Armored => 1.8f,
            LootableObjectQuality.MilitaryGrade => 2.0f,
            _ => 1.0f
        };
    }

    private float GetEngineTorque(LootableObjectQuality quality)
    {
        return quality switch
        {
            LootableObjectQuality.Normal => 250f,
            LootableObjectQuality.Improved => 350f,
            LootableObjectQuality.Reinforced => 450f,
            LootableObjectQuality.Armored => 550f,
            LootableObjectQuality.MilitaryGrade => 650f,
            _ => 250f
        };
    }

    private float GetBodyAirResistance(LootableObjectQuality quality)
    {
        return quality switch
        {
            LootableObjectQuality.Normal => 30f,
            LootableObjectQuality.Improved => 50f,
            LootableObjectQuality.Reinforced => 70f,
            LootableObjectQuality.Armored => 90f,
            LootableObjectQuality.MilitaryGrade => 100f,
            _ => 30f
        };
    }

    private float GetLightIntensity(LootableObjectQuality quality)
    {
        return quality switch
        {
            LootableObjectQuality.Normal => 500f,
            LootableObjectQuality.Improved => 750f,
            LootableObjectQuality.Reinforced => 1000f,
            LootableObjectQuality.Armored => 1250f,
            LootableObjectQuality.MilitaryGrade => 1500f,
            _ => 500f
        };
    }

    private float GetBrakeTorque(LootableObjectQuality quality)
    {
        return quality switch
        {
            LootableObjectQuality.Normal => 250f,
            LootableObjectQuality.Improved => 300f,
            LootableObjectQuality.Reinforced => 350f,
            LootableObjectQuality.Armored => 400f,
            LootableObjectQuality.MilitaryGrade => 450f,
            _ => 250f
        };
    }

    private float GetSuspensionDistance(LootableObjectQuality quality)
    {
        return quality switch
        {
            LootableObjectQuality.Normal => 0.1f,
            LootableObjectQuality.Improved => 0.12f,
            LootableObjectQuality.Reinforced => 0.13f,
            LootableObjectQuality.Armored => 0.14f,
            LootableObjectQuality.MilitaryGrade => 0.15f,
            _ => 0.1f
        };
    }
}

public enum LootableObjectType
{
    Tires,
    Engine,
    Body,
    Light,
    Brakes,
    Suspension
}

public enum LootableObjectQuality
{
    Normal,
    Improved,
    Reinforced,
    Armored,
    MilitaryGrade
}