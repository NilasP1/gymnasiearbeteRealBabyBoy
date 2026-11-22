
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public Canvas inventoryCanvas;
    public GameObject inventoryPanel;

    [Header("Inventory Background Effect")]
    public Volume inventoryVolume;

    [Header("Panel Animation")]
    [Range(0f, 0.8f)]
    public float startingYScale = 0.2f;

    public float scaleSpeed = 4f;
    public float targetScale = 0.8f;

    public bool isInventoryOpen;

    private RectTransform panelRect;
    private bool isAnimating;
    private bool isOpening;

    private void Start()
    {
        if (inventoryPanel != null)
        {
            panelRect = inventoryPanel.GetComponent<RectTransform>();
            panelRect.localScale = Vector3.zero;
            inventoryPanel.SetActive(false);
        }

        if (inventoryCanvas != null)
            inventoryCanvas.enabled = false;

        if (inventoryVolume != null)
            inventoryVolume.enabled = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (!isAnimating || panelRect == null)
            return;

        Vector3 scale = panelRect.localScale;

        if (isOpening)
        {
            // Stage 1: Increase X first.
            if (scale.x < targetScale)
            {
                scale.x = Mathf.MoveTowards(
                    scale.x,
                    targetScale,
                    scaleSpeed * Time.deltaTime
                );
            }
            // Stage 2: Increase Y after X reaches its target.
            else
            {
                scale.y = Mathf.MoveTowards(
                    scale.y,
                    targetScale,
                    scaleSpeed * Time.deltaTime
                );
            }

            if (scale.x >= targetScale && scale.y >= targetScale)
                isAnimating = false;
        }
        else
        {
            // Stage 1: Decrease Y first.
            if (scale.y > 0f)
            {
                scale.y = Mathf.MoveTowards(
                    scale.y,
                    0f,
                    scaleSpeed * Time.deltaTime
                );
            }
            // Stage 2: Decrease X after Y reaches zero.
            else
            {
                scale.x = Mathf.MoveTowards(
                    scale.x,
                    0f,
                    scaleSpeed * Time.deltaTime
                );
            }

            if (scale.x <= 0f && scale.y <= 0f)
            {
                isAnimating = false;

                if (inventoryPanel != null)
                    inventoryPanel.SetActive(false);

                if (inventoryCanvas != null)
                    inventoryCanvas.enabled = false;
            }
        }

        panelRect.localScale = scale;
    }

    public void OpenAndCloseInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        isInventoryOpen = !isInventoryOpen;
        isOpening = isInventoryOpen;

        isAnimating = true;

        if (playerMovement != null)
            playerMovement.enabled = !isInventoryOpen;

        Cursor.visible = isInventoryOpen;

        Cursor.lockState = isInventoryOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        if (isOpening)
        {
            if (inventoryCanvas != null)
                inventoryCanvas.enabled = true;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);

                panelRect.localScale = new Vector3(
                    0f,
                    startingYScale,
                    1f
                );
            }
        }

        if (inventoryVolume != null)
            inventoryVolume.enabled = isInventoryOpen;
    }
}
