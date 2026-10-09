
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Items")]
    public List<GameObject> items = new List<GameObject>();

    [Header("Inventory UI")]
    public List<GameObject> InventoryUISlots = new List<GameObject>();
    public List<GameObject> InventoryUIbuttons = new List<GameObject>();
    public List<Sprite> itemSprites = new List<Sprite>();

    [Header("References")]
    public PlayerMovement PlayerMovement;
    public Canvas inventoryCanvas;
    public CarUpgrading carUpgrading;

    public bool isInventoryOpen;

    private void Start()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.enabled = false;

        RefreshInventoryUI();
    }

    // Call this method from your existing pickup interaction.
    public void AddItem(GameObject item)
    {
        if (item == null)
            return;

        if (item.GetComponent<LootableObject>() == null)
        {
            Debug.LogWarning(
                item.name + " has no LootableObject component.");
            return;
        }

        if (items.Contains(item))
            return;

        if (items.Count >= InventoryUISlots.Count)
        {
            Debug.LogWarning("Inventory is full.");
            return;
        }

        // Store the actual item GameObject.
        items.Add(item);

        // Keep inventory items out of the world while stored.
        item.transform.SetParent(transform, false);
        item.SetActive(false);

        RefreshInventoryUI();

        Debug.Log("Added " + item.name + " to inventory.");
    }

 
    public void EquipItemPressedOnInInventory()
    {
        // Find which inventory button was pressed.
        int index = InventoryUIbuttons.IndexOf(
            UnityEngine.EventSystems.EventSystem.current
                .currentSelectedGameObject
        );

        // Make sure the button corresponds to an inventory item.
        if (index < 0 || index >= items.Count)
            return;

        // Get the selected inventory item.
        GameObject selectedObject = items[index];

        if (selectedObject == null)
            return;

        LootableObject selectedPart =
            selectedObject.GetComponent<LootableObject>();

        if (selectedPart == null)
        {
            Debug.LogWarning("Selected inventory item has no LootableObject component.");
            return;
        }

        // Equip the selected part and get the part it replaced.
        carUpgrading.EquipCarPart(selectedPart);

        LootableObject oldPart = carUpgrading.DeEquipedItem;

        // Remove the selected item from the inventory list.
        items.RemoveAt(index);

        // Add the previously equipped part to the inventory.
        if (oldPart != null)
        {
            AddItem(oldPart.gameObject);
        }

        // Refresh the inventory buttons and icons.
        RefreshInventoryUI();
    }


    private void RefreshInventoryUI()
    {
        // Clear all slot images.
        for (int i = 0; i < InventoryUISlots.Count; i++)
        {
            Image image = GetSlotImage(i);

            if (image != null)
                image.sprite = null;
        }

        // Draw the icons for current inventory items.
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                continue;

            LootableObject part =
                items[i].GetComponent<LootableObject>();

            if (part == null)
                continue;

            Image image = GetSlotImage(i);

            if (image == null)
                continue;

            int spriteIndex = GetSpriteIndex(
                part.lootableObjectType,
                part.lootableObjectQuality);

            if (spriteIndex >= 0 && spriteIndex < itemSprites.Count)
                image.sprite = itemSprites[spriteIndex];
        }
    }

    private int GetSpriteIndex(
        LootableObjectType type,
        LootableObjectQuality quality)
    {
        return ((int)type * 5) + (int)quality;
    }

    private Image GetSlotImage(int index)
    {
        if (index < 0 || index >= InventoryUISlots.Count)
            return null;

        GameObject slot = InventoryUISlots[index];

        if (slot == null || slot.transform.childCount == 0)
            return null;

        Transform firstChild = slot.transform.GetChild(0);

        if (firstChild.childCount == 0)
            return null;

        return firstChild.GetChild(0).GetComponent<Image>();
    }

    public void OpenAndCloseInventory(
        InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        isInventoryOpen = !isInventoryOpen;

        if (PlayerMovement != null)
            PlayerMovement.enabled = !isInventoryOpen;

        Cursor.visible = isInventoryOpen;
        Cursor.lockState = isInventoryOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        if (inventoryCanvas != null)
            inventoryCanvas.enabled = isInventoryOpen;
    }
}