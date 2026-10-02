using UnityEngine;

public class LootableObject : MonoBehaviour
{
    public LootableObjectType lootableObjectType;
    public LootableObjectQuality lootableObjectQuality;

}

public enum LootableObjectType
{
    Tires,
    Engine,
    Body,
    Light,
    Brakes,
    Suspension,

}

public enum LootableObjectQuality
{
    Normal,
    Improved,
    Reinforced,
    Armored,
    MilitaryGrade
}
