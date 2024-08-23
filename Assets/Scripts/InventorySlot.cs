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
        if (inventorySystem != null)
        {
            gridLayoutGroup = inventorySystem.inventoryPanel.GetComponent<GridLayoutGroup>();
        }
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

        // Manually sort all slots except the one being dragged
        AutoSortInventoryExceptThis();
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
            // Reparent and reposition the item back to the closest grid slot
            Transform closestSlot = GetClosestSlot(localPoint);
            if (closestSlot != null)
            {
                rectTransform.SetParent(closestSlot);
                rectTransform.anchoredPosition = Vector2.zero;
            }
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
            gridLayoutGroup.SetLayoutHorizontal();
            gridLayoutGroup.SetLayoutVertical();
        }

        // Auto-sort inventory after drag ends
        AutoSortInventory();
    }

    private Transform GetClosestSlot(Vector2 localPoint)
    {
        float closestDistance = float.MaxValue;
        Transform closestSlot = null;

        foreach (Transform slot in inventorySystem.inventoryPanel.transform)
        {
            float distance = Vector2.Distance(localPoint, slot.localPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        }

        return closestSlot;
    }

    private void ResetItemPosition()
    {
        // Reset position to the original grid slot
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;
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
            int newTier = itemTier + 1;
            string newItemName = "T" + newTier + itemType.ToString();

            GameObject newItemPrefab = Resources.Load<GameObject>(newItemName);
            if (newItemPrefab != null)
            {
                DroppedEquipment newDroppedEquipment = newItemPrefab.GetComponent<DroppedEquipment>();

                if (newDroppedEquipment != null)
                {
                    GetComponent<Image>().sprite = newDroppedEquipment.inventorySprite;
                    itemName = newItemName;
                    itemTier = newTier;

                    // Remove old items and add the new merged item
                    inventorySystem.inventoryItems.RemoveAll(item =>
                        (item.name == otherSlot.itemName && item.tier == otherSlot.itemTier) ||
                        (item.name == itemName && item.tier == itemTier && item.name != itemName));

                    inventorySystem.inventoryItems.Add(new InventorySystem.InventoryItem
                    {
                        name = newItemName,
                        tier = newTier,
                        type = itemType,
                        spriteName = newDroppedEquipment.inventorySprite.name
                    });

                    inventorySystem.UpdateInventorySlot(this);

                    Destroy(otherSlot.gameObject);

                    inventorySystem.SaveInventory();

                    // Re-enable GridLayoutGroup after merging
                    if (gridLayoutGroup != null)
                    {
                        gridLayoutGroup.enabled = true;
                    }

                    // Auto-sort inventory after merging
                    AutoSortInventory();

                    return;
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

    private void AutoSortInventoryExceptThis()
    {
        // Sort all slots except the dragged one
        for (int i = 0; i < inventorySystem.inventoryPanel.transform.childCount; i++)
        {
            Transform child = inventorySystem.inventoryPanel.transform.GetChild(i);
            if (child != transform) // Skip the dragged item
            {
                child.SetSiblingIndex(i);
            }
        }
    }

    private void AutoSortInventory()
    {
        // Sort all slots in the inventory
        for (int i = 0; i < inventorySystem.inventoryPanel.transform.childCount; i++)
        {
            Transform child = inventorySystem.inventoryPanel.transform.GetChild(i);
            child.SetSiblingIndex(i);
        }
    }
}