using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> InventoryUISlots = new List<GameObject>();
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
        Image inventorySlotImage;
        int slotIndex = items.IndexOf(item);
        inventorySlotImage = InventoryUISlots[slotIndex].transform.GetChild(0).GetComponentInChildren<Image>();

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
