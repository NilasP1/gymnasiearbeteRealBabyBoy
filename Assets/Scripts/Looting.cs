
using UnityEngine;

public class Looting : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;

    public void LootingItem(GameObject lootObject)
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned!");
            return;
        }

        inventoryManager.TryAddLoot(lootObject);
    }
}
