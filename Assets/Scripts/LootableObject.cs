using UnityEngine;

public class LootableObject : MonoBehaviour
{
    public LootableObjectType lootableObjectType;
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
