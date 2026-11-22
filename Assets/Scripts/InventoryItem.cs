
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Grid Size")]
    public int width = 2;
    public int height = 2;

    [Header("Template Information")]
    public LootableObjectType templateType;
    public LootableObjectQuality templateQuality;

    [Header("Actual Part Data")]
    public LootableObject partData;

    [HideInInspector] public InventoryManager inventory;
    [HideInInspector] public int gridX;
    [HideInInspector] public int gridY;
    [HideInInspector] public bool isPlaced;
    [HideInInspector] public bool wasEquipped;

    private RectTransform rect;
    private Vector2 dragOffset;
    private Image itemImage;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        itemImage = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventory == null || !isPlaced)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventory.itemContainer,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        dragOffset = localPoint - rect.anchoredPosition;

        if (itemImage != null)
            itemImage.raycastTarget = false;

        rect.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventory == null || !isPlaced)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventory.itemContainer,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        rect.anchoredPosition = localPoint - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemImage != null)
            itemImage.raycastTarget = true;

        if (wasEquipped)
        {
            wasEquipped = false;
            return;
        }

        if (inventory == null || !isPlaced)
            return;

        // Check whether the item was released outside the inventory panel.
        bool insideInventory = inventory.IsPointerInsideInventoryPanel(
            eventData.position,
            eventData.pressEventCamera
        );

        if (!insideInventory)
        {
            // Drop the item into the world.
            if (inventory.TryDropItemToWorld(this))
                return;

            // If dropping fails, restore its original position.
            inventory.RestoreItemPosition(this);
            return;
        }

        // Otherwise, attempt to move the item inside the grid.
        float step = inventory.cellSize + inventory.spacing;

        Vector2 pos = rect.anchoredPosition;

        int x = Mathf.RoundToInt(pos.x / step);
        int y = Mathf.RoundToInt(-pos.y / step);

        inventory.TryMoveItem(this, x, y);
    }
}
