
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 6;
    public float cellSize = 64;
    public float spacing = 2;

    [Header("World Dropping")]
    public RectTransform inventoryPanelRect;
    public Transform dropPoint;
    public float dropDistance = 2f;

    [Header("UI References")]
    public RectTransform gridRect;
    public RectTransform itemContainer;

    [Header("Item Templates")]
    public InventoryItem[] itemTemplates;

    [Header("Cell Colors")]
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f);
    public Color occupiedColor = new Color(0.45f, 0.45f, 0.45f);

    private InventoryItem[,] grid;
    private Image[,] cellImages;

    private void Awake()
    {
        grid = new InventoryItem[width, height];
        cellImages = new Image[width, height];

        if (gridRect == null || itemContainer == null)
        {
            Debug.LogError("Assign Grid Rect and Item Container!");
            return;
        }

        if (gridRect.childCount < width * height)
        {
            Debug.LogError("InventoryGrid needs at least " +
                (width * height) + " Cell children.");
            return;
        }

        for (int i = 0; i < width * height; i++)
        {
            int x = i % width;
            int y = i / width;

            cellImages[x, y] =
                gridRect.GetChild(i).GetComponent<Image>();

            if (cellImages[x, y] == null)
                Debug.LogError("Cell " + i + " has no Image!");
        }

        UpdateCellColors();
    }

    public bool TryAddLoot(GameObject lootObject)
    {
        if (lootObject == null)
            return false;

        LootableObject part =
            lootObject.GetComponent<LootableObject>();

        if (part == null)
            return false;

        if (!TryAddPartData(part))
            return false;

        lootObject.SetActive(false);

        Debug.Log("Looted: " + lootObject.name);

        return true;
    }

    public bool TryAddPartData(LootableObject part)
    {
        if (part == null)
            return false;

        InventoryItem template = FindTemplate(
            part.lootableObjectType,
            part.lootableObjectQuality
        );

        if (template == null)
        {
            Debug.LogWarning("Missing inventory template!");
            return false;
        }

        if (!FindFirstAvailablePosition(
            template.width,
            template.height,
            null,
            out int x,
            out int y))
        {
            Debug.LogWarning("Inventory is full!");
            return false;
        }

        return CreateInventoryItem(template, part, x, y);
    }

    public bool IsPointerInsideInventoryPanel(
    Vector2 screenPosition,
    Camera eventCamera)
    {
        RectTransform panel = inventoryPanelRect != null
            ? inventoryPanelRect
            : gridRect;

        return RectTransformUtility.RectangleContainsScreenPoint(
            panel,
            screenPosition,
            eventCamera
        );
    }

    public bool TryDropItemToWorld(InventoryItem item)
    {
        if (item == null || item.partData == null || !item.isPlaced)
            return false;

        LootableObject part = item.partData;

        Transform spawnTransform = dropPoint != null
            ? dropPoint
            : Camera.main != null ? Camera.main.transform : null;

        if (spawnTransform == null)
        {
            Debug.LogWarning("No drop point or Main Camera assigned!");
            return false;
        }

        Vector3 dropPosition = dropPoint != null
            ? dropPoint.position
            : spawnTransform.position + spawnTransform.forward * dropDistance;

        // Remove the item from the inventory grid.
        ClearGridReferences(item);

        // Return the original LootableObject to the world.
        part.transform.SetParent(null, true);
        part.transform.position = dropPosition;
        part.gameObject.SetActive(true);

        // Remove the inventory UI representation.
        Destroy(item.gameObject);

        UpdateCellColors();

        Debug.Log("Dropped item into the world: " + part.name);

        return true;
    }
    private InventoryItem FindTemplate(
        LootableObjectType type,
        LootableObjectQuality quality)
    {
        foreach (InventoryItem template in itemTemplates)
        {
            if (template == null)
                continue;

            if (template.templateType == type &&
                template.templateQuality == quality)
                return template;
        }

        return null;
    }

    public InventoryItem GetTemplateForPart(LootableObject part)
    {
        if (part == null)
            return null;

        return FindTemplate(
            part.lootableObjectType,
            part.lootableObjectQuality
        );
    }

    private bool CreateInventoryItem(
        InventoryItem template,
        LootableObject part,
        int x,
        int y)
    {
        InventoryItem newItem = Instantiate(
            template,
            itemContainer
        );

        newItem.gameObject.SetActive(true);
        newItem.inventory = this;
        newItem.partData = part;
        newItem.wasEquipped = false;
        newItem.isPlaced = false;

        if (!TryPlace(newItem, x, y))
        {
            Destroy(newItem.gameObject);
            return false;
        }

        return true;
    }

    public bool CanPlace(
        InventoryItem item,
        int x,
        int y,
        InventoryItem ignoreItem = null)
    {
        if (item == null)
            return false;

        if (x < 0 || y < 0 ||
            x + item.width > width ||
            y + item.height > height)
            return false;

        for (int ix = x; ix < x + item.width; ix++)
        {
            for (int iy = y; iy < y + item.height; iy++)
            {
                InventoryItem occupied = grid[ix, iy];

                if (occupied != null &&
                    occupied != item &&
                    occupied != ignoreItem)
                    return false;
            }
        }

        return true;
    }

    public bool TryPlace(InventoryItem item, int x, int y)
    {
        if (!CanPlace(item, x, y))
            return false;

        ClearGridReferences(item);

        for (int ix = x; ix < x + item.width; ix++)
        {
            for (int iy = y; iy < y + item.height; iy++)
                grid[ix, iy] = item;
        }

        item.gridX = x;
        item.gridY = y;
        item.isPlaced = true;

        SetGridPosition(item, x, y);
        UpdateCellColors();

        return true;
    }

    public bool TryMoveItem(InventoryItem item, int x, int y)
    {
        if (item == null)
            return false;

        if (!CanPlace(item, x, y, item))
        {
            RestoreItemPosition(item);
            return false;
        }

        return TryPlace(item, x, y);
    }

    public void RestoreItemPosition(InventoryItem item)
    {
        if (item == null || !item.isPlaced)
            return;

        SetGridPosition(item, item.gridX, item.gridY);
    }

    private void SetGridPosition(
        InventoryItem item,
        int x,
        int y)
    {
        float step = cellSize + spacing;

        RectTransform rect =
            item.GetComponent<RectTransform>();

        rect.anchoredPosition =
            new Vector2(x * step, -y * step);

        rect.sizeDelta = new Vector2(
            item.width * cellSize + (item.width - 1) * spacing,
            item.height * cellSize + (item.height - 1) * spacing
        );
    }

    private void ClearGridReferences(InventoryItem item)
    {
        if (item == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == item)
                    grid[x, y] = null;
            }
        }

        item.isPlaced = false;
    }

    public void ClearItem(InventoryItem item)
    {
        if (item == null)
            return;

        ClearGridReferences(item);
        UpdateCellColors();
    }

    private bool FindFirstAvailablePosition(
        int itemWidth,
        int itemHeight,
        InventoryItem ignoreItem,
        out int resultX,
        out int resultY)
    {
        resultX = -1;
        resultY = -1;

        for (int y = 0; y <= height - itemHeight; y++)
        {
            for (int x = 0; x <= width - itemWidth; x++)
            {
                bool canFit = true;

                for (int ix = x; ix < x + itemWidth; ix++)
                {
                    for (int iy = y; iy < y + itemHeight; iy++)
                    {
                        InventoryItem occupied = grid[ix, iy];

                        if (occupied != null &&
                            occupied != ignoreItem)
                        {
                            canFit = false;
                            break;
                        }
                    }

                    if (!canFit)
                        break;
                }

                if (canFit)
                {
                    resultX = x;
                    resultY = y;
                    return true;
                }
            }
        }

        return false;
    }

    public bool TryEquipItem(
        InventoryItem item,
        LootableObjectType acceptedType,
        CarUpgrading carUpgrading)
    {
        if (item == null || item.partData == null ||
            carUpgrading == null)
            return false;

        LootableObject newPart = item.partData;

        if (newPart.lootableObjectType != acceptedType)
            return false;

        LootableObject oldPart = GetEquippedPart(
            acceptedType,
            carUpgrading
        );

        InventoryItem oldTemplate = null;
        int oldX = 0;
        int oldY = 0;

        if (oldPart != null)
        {
            oldTemplate = FindTemplate(
                oldPart.lootableObjectType,
                oldPart.lootableObjectQuality
            );

            if (oldTemplate == null)
                return false;

            if (!FindFirstAvailablePosition(
                oldTemplate.width,
                oldTemplate.height,
                item,
                out oldX,
                out oldY))
            {
                Debug.LogWarning("Not enough inventory space!");
                return false;
            }
        }

        if (!carUpgrading.TryEquipCarPart(
            newPart,
            out LootableObject returnedPart))
            return false;

        ClearGridReferences(item);

        item.wasEquipped = true;
        Destroy(item.gameObject);

        if (returnedPart != null)
        {
            if (!CreateInventoryItem(
                oldTemplate,
                returnedPart,
                oldX,
                oldY))
            {
                Debug.LogError("Could not return old equipment!");
                return false;
            }
        }

        UpdateCellColors();

        Debug.Log("Equipped: " + acceptedType);

        return true;
    }

    public bool TryUnequipItem(
        EquipmentSlot slot,
        int x,
        int y)
    {
        if (slot == null || slot.carUpgrading == null)
            return false;

        LootableObject part = GetEquippedPart(
            slot.acceptedType,
            slot.carUpgrading
        );

        if (part == null)
            return false;

        InventoryItem template = FindTemplate(
            part.lootableObjectType,
            part.lootableObjectQuality
        );

        if (template == null)
            return false;

        if (!CanPlace(template, x, y))
        {
            Debug.Log("Not enough inventory space!");
            return false;
        }

        LootableObject removedPart =
            slot.carUpgrading.TakeCarPart(slot.acceptedType);

        if (removedPart == null)
            return false;

        if (!CreateInventoryItem(template, removedPart, x, y))
        {
            slot.carUpgrading.TryEquipCarPart(
                removedPart,
                out _
            );

            return false;
        }

        slot.ClearEquippedItem();

        UpdateCellColors();

        Debug.Log("Unequipped: " + slot.acceptedType);

        return true;
    }

    public LootableObject GetEquippedPart(LootableObjectType type, CarUpgrading car)
    {
        if (car == null)
            return null;

        switch (type)
        {
            case LootableObjectType.Tires:
                return car.EquippedCarTires;

            case LootableObjectType.Engine:
                return car.EquippedCarEngine;

            case LootableObjectType.Body:
                return car.EquippedCarBody;

            case LootableObjectType.Light:
                return car.EquippedCarLights;

            case LootableObjectType.Brakes:
                return car.EquippedCarBrakes;

            case LootableObjectType.Suspension:
                return car.EquippedCarSuspension;
        }

        return null;
    }

    public void UpdateCellColors()
    {
        if (cellImages == null || grid == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (cellImages[x, y] == null)
                    continue;

                cellImages[x, y].color =
                    grid[x, y] != null
                    ? occupiedColor
                    : emptyColor;
            }
        }
    }
}
