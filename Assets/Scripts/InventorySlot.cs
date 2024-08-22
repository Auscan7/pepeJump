using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public string itemName;
    public int itemTier;
    public DroppedEquipment.EquipmentType itemType;

    private Transform originalParent;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventorySystem inventorySystem;
    private GridLayoutGroup gridLayoutGroup;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        inventorySystem = FindObjectOfType<InventorySystem>();
        gridLayoutGroup = inventorySystem.inventoryPanel.GetComponent<GridLayoutGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return; // Prevent dragging if the slot is empty

        originalParent = transform.parent;
        transform.SetParent(inventorySystem.inventoryPanel.transform); // Keep within the inventory panel
        transform.SetAsLastSibling(); // Move the item to the top of the UI hierarchy
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent during drag

        // Disable GridLayoutGroup temporarily
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.enabled = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        rectTransform.position = eventData.position; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Check if the item is dropped within the bounds of the inventory panel
        RectTransform panelRect = (RectTransform)inventorySystem.inventoryPanel.transform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, Input.mousePosition, null, out localPoint);

        if (panelRect.rect.Contains(localPoint))
        {
            // If dropped inside the inventory panel, find the closest slot
            transform.SetParent(originalParent);

            // Reposition the item based on grid layout
            Vector2 slotPosition = GetSlotPositionAtLocalPoint(localPoint, panelRect);
            rectTransform.anchoredPosition = slotPosition;

            // Update grid layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
        }
        else
        {
            // Return item to original position if dropped outside
            ResetItemPosition();
        }

        // Re-enable GridLayoutGroup after dragging
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.enabled = true;
        }
    }

    // Helper method to get the position of a slot based on local point
    private Vector2 GetSlotPositionAtLocalPoint(Vector2 localPoint, RectTransform panelRect)
    {
        // Calculate the slot position based on the localPoint
        Vector2 slotSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;

        // Convert localPoint to slot position
        float x = Mathf.Floor((localPoint.x + panelRect.rect.width / 2) / (slotSize.x + spacing.x)) * (slotSize.x + spacing.x) - panelRect.rect.width / 2;
        float y = Mathf.Floor((localPoint.y + panelRect.rect.height / 2) / (slotSize.y + spacing.y)) * (slotSize.y + spacing.y) - panelRect.rect.height / 2;

        return new Vector2(x, y);
    }

    private void ResetItemPosition()
    {
        // Reset position to the original grid slot
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)inventorySystem.inventoryPanel.transform);
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

        if (draggedSlot != null && draggedSlot != this)
        {
            MergeItems(draggedSlot);
        }
    }

    private void MergeItems(InventorySlot otherSlot)
    {
        // Check if they are the same type and tier
        if (otherSlot.itemType == itemType && otherSlot.itemTier == itemTier)
        {
            // Merge items: Create a new item with a higher tier
            int newTier = itemTier + 1;
            string newItemName = "T" + newTier + itemType.ToString();  // Adjusted format to match your naming convention

            // Find and load the new item prefab
            GameObject newItemPrefab = Resources.Load<GameObject>(newItemName);
            if (newItemPrefab != null)
            {
                DroppedEquipment newDroppedEquipment = newItemPrefab.GetComponent<DroppedEquipment>();

                if (newDroppedEquipment != null)
                {
                    // Update this slot with the merged item
                    GetComponent<Image>().sprite = newDroppedEquipment.inventorySprite;
                    itemName = newItemName;
                    itemTier = newTier;

                    // Log the updated item details
                    Debug.Log($"Merged Item - New Item: {newItemName}, Tier: {newTier}, Type: {itemType}, SpriteName: {newDroppedEquipment.inventorySprite.name}");

                    // Remove only the items that are being merged
                    inventorySystem.inventoryItems.RemoveAll(item =>
                        (item.name == otherSlot.itemName && item.tier == otherSlot.itemTier) ||
                        (item.name == itemName && item.tier == itemTier && item.name != itemName)); // Ensure it doesn't remove the newly added item

                    // Add new item
                    inventorySystem.inventoryItems.Add(new InventorySystem.InventoryItem
                    {
                        name = newItemName,
                        tier = newTier,
                        type = itemType,
                        spriteName = newDroppedEquipment.inventorySprite.name
                    });

                    // Update the inventory slot
                    inventorySystem.UpdateInventorySlot(this);

                    // Destroy the other slot's game object
                    Destroy(otherSlot.gameObject);

                    // Log after merging and saving
                    Debug.Log("Inventory before saving: " + string.Join(", ", inventorySystem.inventoryItems.Select(item => $"{item.name} (Tier: {item.tier})")));

                    // Save inventory
                    inventorySystem.SaveInventory();

                    // Log after saving
                    Debug.Log("Inventory after merging and saving: " + string.Join(", ", inventorySystem.inventoryItems.Select(item => $"{item.name} (Tier: {item.tier})")));

                    return; // Exit after successful merge
                }
                else
                {
                    Debug.LogWarning($"DroppedEquipment component not found in {newItemName} prefab.");
                }
            }
            else
            {
                Debug.LogWarning($"Prefab {newItemName} not found in Resources.");
            }
        }
        else
        {
            Debug.Log("Items are not the same type and tier, cannot merge.");
        }
    }
}
