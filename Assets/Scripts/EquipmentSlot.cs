
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour,
    IDropHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    public LootableObjectType acceptedType;
    public InventoryManager inventory;
    public CarUpgrading carUpgrading;

    [Header("Slot Image Settings")]
    public Sprite defaultImage;

    [Range(0, 255)]
    public int EquipedItemImageAlpha = 255;

    [Range(0, 255)]
    public int UnequipedItemImageAlpha = 100;

    private Image slotIcon;
    private LootableObject currentPart;

    private RectTransform iconRect;
    private Vector2 originalIconPosition;
    private Vector2 dragOffset;

    private void Awake()
    {
        Transform icon = transform.Find("Icon");

        if (icon != null)
        {
            slotIcon = icon.GetComponent<Image>();
            iconRect = icon.GetComponent<RectTransform>();
        }
    }

    private void Start()
    {
        if (inventory == null || carUpgrading == null)
            return;

        currentPart = inventory.GetEquippedPart(
            acceptedType,
            carUpgrading
        );

        if (currentPart != null)
        {
            InventoryItem template =
                inventory.GetTemplateForPart(currentPart);

            if (template != null)
            {
                Image image = template.GetComponent<Image>();

                if (image != null)
                {
                    SetEquippedImage(image.sprite);
                    return;
                }
            }
        }

        SetDefaultImage();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null)
            return;

        InventoryItem item =
            draggedObject.GetComponent<InventoryItem>();

        if (item == null || item.partData == null)
            return;

        if (item.partData.lootableObjectType != acceptedType)
            return;

        Image itemImage = item.GetComponent<Image>();

        if (itemImage == null)
            return;

        Sprite sprite = itemImage.sprite;
        LootableObject part = item.partData;

        if (inventory.TryEquipItem(
            item,
            acceptedType,
            carUpgrading))
        {
            currentPart = part;
            SetEquippedImage(sprite);
        }
    }

    private void SetEquippedImage(Sprite sprite)
    {
        if (slotIcon == null)
            return;

        slotIcon.sprite = sprite;
        slotIcon.enabled = true;

        SetAlpha(EquipedItemImageAlpha);
    }

    private void SetDefaultImage()
    {
        if (slotIcon == null)
            return;

        slotIcon.sprite = defaultImage;
        slotIcon.enabled = defaultImage != null;

        SetAlpha(UnequipedItemImageAlpha);
    }

    public void ClearEquippedItem()
    {
        currentPart = null;
        SetDefaultImage();
    }

    private void SetAlpha(int alpha)
    {
        if (slotIcon == null)
            return;

        Color color = slotIcon.color;
        color.a = alpha / 255f;
        slotIcon.color = color;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentPart == null || iconRect == null)
            return;

        originalIconPosition = iconRect.anchoredPosition;

        RectTransform parent =
            iconRect.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        dragOffset = localPoint - originalIconPosition;

        slotIcon.raycastTarget = false;

        iconRect.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentPart == null || iconRect == null)
            return;

        RectTransform parent =
            iconRect.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        iconRect.anchoredPosition = localPoint - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (slotIcon != null)
            slotIcon.raycastTarget = true;

        if (iconRect != null)
            iconRect.anchoredPosition = originalIconPosition;
    }
}
