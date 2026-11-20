
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryGridDropTarget : MonoBehaviour, IDropHandler
{
    public InventoryManager inventory;

    public void OnDrop(PointerEventData eventData)
    {
        if (inventory == null)
            return;

        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null)
            return;

        EquipmentSlot slot = draggedObject.GetComponent<EquipmentSlot>();

        if (slot == null)
            return;

        RectTransform gridRect = transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        float step = inventory.cellSize + inventory.spacing;

        int x = Mathf.FloorToInt(localPoint.x / step);
        int y = Mathf.FloorToInt(-localPoint.y / step);

        inventory.TryUnequipItem(slot, x, y);
    }
}
