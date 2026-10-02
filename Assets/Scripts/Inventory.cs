using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> InventoryUISlots = new List<GameObject>();
    public List<Sprite> itemSprites = new List<Sprite>();
    public bool isInventoryOpen = false;
    public PlayerMovement PlayerMovement;
    public Canvas inventoryCanvas;

    private void Start()
    {
        if (inventoryCanvas != null)
        {
            inventoryCanvas.enabled = false; // Ensure the inventory canvas is hidden at the start
        }
    }

    public void AddItem(GameObject item)
    {
        items.Add(item);
        UnityEngine.UI.Image inventorySlotImage;
        int slotIndex = items.IndexOf(item);
        inventorySlotImage = InventoryUISlots[slotIndex].transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>(); // Get the Image component of the first child of the first child of the inventory slot
        Sprite itemSprite = null;

        LootableObjectType ItemType = item.GetComponent<LootableObject>().lootableObjectType;
        LootableObjectQuality ItemQuality = item.GetComponent<LootableObject>().lootableObjectQuality; 

        switch (ItemType)
        {
            case LootableObjectType.Tires:
                switch (ItemQuality)
                {
                    case LootableObjectQuality.Normal:
                        // Set the image for Normal Tires
                        itemSprite = itemSprites[0];
                        break;
                    case LootableObjectQuality.Improved:
                        // Set the image for Improved Tires
                        itemSprite = itemSprites[1];
                        break;
                    case LootableObjectQuality.Reinforced:
                        // Set the image for Reinforced Tires
                        itemSprite = itemSprites[2];
                        break;
                    case LootableObjectQuality.Armored:
                        // Set the image for Armored Tires
                        itemSprite = itemSprites[3];
                        break;
                    case LootableObjectQuality.MilitaryGrade:
                        // Set the image for Military Grade Tires
                        itemSprite = itemSprites[4];
                        break;
                }
                break;
            case LootableObjectType.Engine:
                switch (ItemQuality)
                {
                    case LootableObjectQuality.Normal:
                        // Set the image for Normal Engine
                        itemSprite = itemSprites[5];
                        break;
                    case LootableObjectQuality.Improved:
                        // Set the image for Improved Engine
                        itemSprite = itemSprites[6];
                        break;
                    case LootableObjectQuality.Reinforced:
                        // Set the image for Reinforced Engine
                        itemSprite = itemSprites[7];
                        break;
                    case LootableObjectQuality.Armored:
                        // Set the image for Armored Engine
                        itemSprite = itemSprites[8];
                        break;
                    case LootableObjectQuality.MilitaryGrade:
                        // Set the image for Military Grade Engine
                        itemSprite = itemSprites[9];
                        break;
                }
                break;
            case LootableObjectType.Body:
                switch (ItemQuality)
                {
                    case LootableObjectQuality.Normal:
                        // Set the image for Normal Body
                        itemSprite = itemSprites[10];
                        break;
                    case LootableObjectQuality.Improved:
                        // Set the image for Improved Body
                        itemSprite = itemSprites[11];
                        break;
                    case LootableObjectQuality.Reinforced:
                        // Set the image for Reinforced Body
                        itemSprite = itemSprites[12];
                        break;
                    case LootableObjectQuality.Armored:
                        // Set the image for Armored Body
                        itemSprite = itemSprites[13];
                        break;
                    case LootableObjectQuality.MilitaryGrade:
                        // Set the image for Military Grade Body
                        itemSprite = itemSprites[14];
                        break;
                }
                break;
            case LootableObjectType.Light:
                switch (ItemQuality)
                {
                    case LootableObjectQuality.Normal:
                        // Set the image for Normal Light
                        itemSprite = itemSprites[15];
                        break;
                    case LootableObjectQuality.Improved:
                        // Set the image for Improved Light
                        itemSprite = itemSprites[16];
                        break;
                    case LootableObjectQuality.Reinforced:
                        // Set the image for Reinforced Light
                        itemSprite = itemSprites[17];
                        break;
                    case LootableObjectQuality.Armored:
                        // Set the image for Armored Light
                        itemSprite = itemSprites[18];
                        break;
                    case LootableObjectQuality.MilitaryGrade:
                        // Set the image for Military Grade Light
                        itemSprite = itemSprites[19];
                        break;
                }
                break;
            case LootableObjectType.Brakes:
                switch (ItemQuality)
                {
                    case LootableObjectQuality.Normal:
                        // Set the image for Normal Brakes
                        itemSprite = itemSprites[20];
                        break;
                    case LootableObjectQuality.Improved:
                        // Set the image for Improved Brakes
                        itemSprite = itemSprites[21];
                        break;
                    case LootableObjectQuality.Reinforced:
                        // Set the image for Reinforced Brakes
                        itemSprite = itemSprites[22];
                        break;
                    case LootableObjectQuality.Armored:
                        // Set the image for Armored Brakes
                        itemSprite = itemSprites[23];
                        break;
                    case LootableObjectQuality.MilitaryGrade:
                        // Set the image for Military Grade Brakes
                        itemSprite = itemSprites[24];
                        break;
                }
                break;
            case LootableObjectType.Suspension:
                switch (ItemQuality)
                {
                    case LootableObjectQuality.Normal:
                        // Set the image for Normal Suspension
                        itemSprite = itemSprites[25];
                        break;
                    case LootableObjectQuality.Improved:
                        // Set the image for Improved Suspension
                        itemSprite = itemSprites[26];
                        break;
                    case LootableObjectQuality.Reinforced:
                        // Set the image for Reinforced Suspension
                        itemSprite = itemSprites[27];
                        break;
                    case LootableObjectQuality.Armored:
                        // Set the image for Armored Suspension
                        itemSprite = itemSprites[28];
                        break;
                    case LootableObjectQuality.MilitaryGrade:
                        // Set the image for Military Grade Suspension
                        itemSprite = itemSprites[29];
                        break;
                }
                break;
        }

        inventorySlotImage.sprite = itemSprite; // Set the sprite for the inventory slot image

        Debug.Log($"Item {item.name} added to inventory.");
    }
    public void OpenAndCloseInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        isInventoryOpen = !isInventoryOpen;

       if (isInventoryOpen)
       {
            Debug.Log("Inventory opened.");
            // Show inventory
            PlayerMovement.enabled = false; // Disable player movement when inventory is open
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
            if (inventoryCanvas != null)
            {
                inventoryCanvas.enabled = true; // Show the inventory canvas
            }
        }
       else
       {
            Debug.Log("Inventory closed.");
            // Hide inventory
            PlayerMovement.enabled = true; // Enable player movement when inventory is closed
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
            if (inventoryCanvas != null)
            {
                inventoryCanvas.enabled = false; // Hide the inventory canvas
            }
        }
    }
}
