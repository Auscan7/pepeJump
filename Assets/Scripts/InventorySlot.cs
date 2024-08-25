using System;
using System.Collections.Generic;
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

    private GameObject dragImage; // The image used for dragging
    private Canvas dragCanvas; // Canvas for dragging image

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        inventorySystem = FindObjectOfType<InventorySystem>();

        if (inventorySystem == null)
        {
            Debug.LogError("InventorySystem not found in the scene.");
        }
        else
        {
            gridLayoutGroup = inventorySystem.inventoryPanel.GetComponent<GridLayoutGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        originalParent = transform.parent;

        // Create and set up the drag image
        SetupDragImage();

        // Ensure the item has its own CanvasGroup
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Make the item semi-transparent during drag

        // Disable GridLayoutGroup temporarily
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.enabled = false;
        }
    }

    private void SetupDragImage()
    {
        // Create a new Canvas for the drag image if it doesn't exist
        if (dragCanvas == null)
        {
            dragCanvas = new GameObject("DragCanvas").AddComponent<Canvas>();
            dragCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            dragCanvas.sortingOrder = 100; // Ensure it’s on top of everything
            Debug.Log("Created new DragCanvas");
        }

        // Ensure dragImage is created once
        if (dragImage == null)
        {
            dragImage = new GameObject("DragImage");
            Image image = dragImage.AddComponent<Image>();
            image.sprite = GetComponent<Image>().sprite;

            RectTransform dragRectTransform = dragImage.GetComponent<RectTransform>();
            dragRectTransform.SetParent(dragCanvas.transform, false);

            // Match the scale of the original image
            dragRectTransform.localScale = rectTransform.localScale;
        }

        dragImage.GetComponent<RectTransform>().position = Input.mousePosition; // Position it under the cursor
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        // Update the position of the drag image
        if (dragImage != null)
        {
            dragImage.GetComponent<RectTransform>().position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemName)) return;

        // Destroy the drag image
        if (dragImage != null)
        {
            Debug.Log("Destroying drag image");
            Destroy(dragImage);
            dragImage = null;
        }

        // Clean up DragCanvas
        CleanUpDragCanvas();

        // Ensure canvasGroup is not null
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }
        else
        {
            Debug.LogError("CanvasGroup is null.");
        }

        if (inventorySystem != null)
        {
            RectTransform panelRect = (RectTransform)inventorySystem.inventoryPanel.transform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRect, Input.mousePosition, null, out localPoint);

            bool isDroppedInside = panelRect.rect.Contains(localPoint);

            if (isDroppedInside)
            {
                InventorySlot targetSlot = GetSlotAtPosition(localPoint);
                if (targetSlot != null && targetSlot != this)
                {
                    Debug.Log("Merging items...");
                    MergeItems(targetSlot);
                }
                else
                {
                    ResetItemPosition();
                }
            }
            else
            {
                ResetItemPosition();
            }

            if (gridLayoutGroup != null)
            {
                gridLayoutGroup.enabled = true;
                gridLayoutGroup.SetLayoutHorizontal();
                gridLayoutGroup.SetLayoutVertical();
            }

            AutoSortInventory();
        }
        else
        {
            Debug.LogError("InventorySystem is null.");
        }
    }

    private void CleanUpDragCanvas()
    {
        Debug.Log("Cleaning up DragCanvas...");
        if (dragCanvas != null)
        {
            Debug.Log("Destroying DragCanvas...");
            Destroy(dragCanvas.gameObject);
            dragCanvas = null;
        }
        else
        {
            Debug.Log("No DragCanvas found to destroy.");
        }
    }

    private InventorySlot GetSlotAtPosition(Vector2 localPoint)
    {
        // Check if the position is over any slot
        foreach (Transform slot in inventorySystem.inventoryPanel.transform)
        {
            RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(slotRectTransform, Input.mousePosition))
            {
                return slot.GetComponent<InventorySlot>();
            }
        }

        return null;
    }

    private void ResetItemPosition()
    {
        // Reset position to the original grid slot
        rectTransform.SetParent(originalParent);
        rectTransform.anchoredPosition = Vector2.zero;

        // Ensure that the CanvasGroup is reset
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // Ensure full opacity for new items
            canvasGroup.blocksRaycasts = true; // Make it interactable
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();

        if (draggedSlot != null && draggedSlot != this)
        {
            Debug.Log("Dropping item...");
            MergeItems(draggedSlot);
        }
    }

    private void MergeItems(InventorySlot otherSlot)
    {
        Debug.Log("MergeItems called.");

        if (otherSlot.itemType == itemType && otherSlot.itemTier == itemTier)
        {
            try
            {
                int newTier = itemTier + 1;
                string newItemName = "T" + newTier + itemType.ToString();

                GameObject newItemPrefab = Resources.Load<GameObject>(newItemName);
                if (newItemPrefab != null)
                {
                    DroppedEquipment newDroppedEquipment = newItemPrefab.GetComponent<DroppedEquipment>();

                    if (newDroppedEquipment != null)
                    {
                        // Update the current slot to the new item
                        GetComponent<Image>().sprite = newDroppedEquipment.inventorySprite;
                        itemName = newItemName;
                        itemTier = newTier;

                        // Find and mark the two items that should be removed
                        var itemsToRemove = new List<InventorySystem.InventoryItem>();
                        foreach (var item in inventorySystem.inventoryItems)
                        {
                            if (item.name == otherSlot.itemName && item.tier == otherSlot.itemTier && item.type == otherSlot.itemType)
                            {
                                itemsToRemove.Add(item);
                                if (itemsToRemove.Count == 2)
                                {
                                    break; // Stop after finding the two items to merge
                                }
                            }
                        }

                        // Remove the two specific items that were merged
                        foreach (var item in itemsToRemove)
                        {
                            inventorySystem.inventoryItems.Remove(item);
                        }

                        // Add the new merged item to the inventory
                        inventorySystem.inventoryItems.Add(new InventorySystem.InventoryItem
                        {
                            name = newItemName,
                            tier = newTier,
                            type = itemType,
                            spriteName = newDroppedEquipment.inventorySprite.name
                        });

                        // Update the UI for the other slot
                        otherSlot.GetComponent<Image>().sprite = inventorySystem.placeholderSprite;
                        otherSlot.itemName = string.Empty;
                        otherSlot.itemTier = 0;
                        otherSlot.itemType = DroppedEquipment.EquipmentType.None;

                        // Clean up DragCanvas for the other slot
                        otherSlot.CleanUpDragCanvas();

                        // Save the inventory after updating
                        inventorySystem.SaveInventory();

                        // Reset the CanvasGroup for the new item
                        if (canvasGroup != null)
                        {
                            canvasGroup.alpha = 1f;
                            canvasGroup.blocksRaycasts = true;
                        }

                        // Reset the CanvasGroup for the other slot
                        CanvasGroup otherCanvasGroup = otherSlot.GetComponent<CanvasGroup>();
                        if (otherCanvasGroup != null)
                        {
                            otherCanvasGroup.alpha = 1f;
                            otherCanvasGroup.blocksRaycasts = true;
                        }
                        else
                        {
                            // Ensure that the other slot has a CanvasGroup component
                            otherCanvasGroup = otherSlot.gameObject.AddComponent<CanvasGroup>();
                            otherCanvasGroup.alpha = 1f;
                            otherCanvasGroup.blocksRaycasts = true;
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
            catch (Exception ex)
            {
                Debug.LogError("Error in MergeItems: " + ex.Message);
            }
        }
        else
        {
            Debug.Log("Items are not the same type and tier, cannot merge.");
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
