using UnityEngine;

public class Looting : MonoBehaviour
{
    [SerializeField] private Inventory inventory;

    public void LootingItem(GameObject lootObject)
    {
        inventory.AddItem(lootObject);
        lootObject.SetActive(false); // Deactivate the looted object
    }
}
