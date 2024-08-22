using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the Panel with the GridLayoutGroup
    public GameObject inventoryPanel; // To add slot prefabs as a child
    public GameObject slotPrefab;  // Reference to the Inventory Slot prefab
    public int inventorySize = 20; // Total number of slots in the inventory

    public Sprite placeholderSprite; // Assign this to your specific placeholder sprite

    private List<GameObject> inventorySlots = new List<GameObject>(); // List to hold all slots
    private List<string> inventoryItems = new List<string>(); // List to store item names for saving/loading

    void Start()
    {
        // Initialize the inventory UI
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);
            slot.AddComponent<InventorySlot>(); // Add InventorySlot script for drag and drop
            inventorySlots.Add(slot);
        }

        // Initially hide the inventory UI
        inventoryUI.SetActive(false);

        // Load inventory items
        LoadInventory();
    }

    public bool AddItem(string itemName, int tier = 1, DroppedEquipment.EquipmentType type = DroppedEquipment.EquipmentType.Weapon)
    {
        // Find the first empty or placeholder slot
        foreach (GameObject slot in inventorySlots)
        {
            Image slotImage = slot.GetComponent<Image>();
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (slotImage.sprite == null || slotImage.sprite == placeholderSprite)
            {
                GameObject itemPrefab = Resources.Load<GameObject>(itemName);
                if (itemPrefab != null)
                {
                    slotImage.sprite = itemPrefab.GetComponent<SpriteRenderer>().sprite; // Set the item sprite
                    inventorySlot.itemName = itemName;
                    inventorySlot.itemTier = tier;
                    inventorySlot.itemType = type;
                    inventoryItems.Add(itemName + "," + tier + "," + type); // Add item name, tier, and type to the list for saving
                    SaveInventory(); // Save inventory after adding an item
                    return true; // Successfully added the item
                }
            }
        }

        // Inventory is full
        Debug.Log("Inventory is full!");
        return false;
    }

    public void ToggleInventory()
    {
        // Toggle the inventory UI on or off
        inventoryUI.SetActive(!inventoryUI.activeSelf);

        // Ensure no UI button stays selected after clicking
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SaveInventory() // Changed from private to public
    {
        // Save each item's name, tier, and type as a string
        List<string> savedItems = new List<string>();
        foreach (GameObject slot in inventorySlots)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            if (inventorySlot.itemName != null)
            {
                savedItems.Add(inventorySlot.itemName + "," + inventorySlot.itemTier + "," + inventorySlot.itemType);
            }
        }

        PlayerPrefs.SetString("InventoryItems", string.Join(";", savedItems));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        string savedItems = PlayerPrefs.GetString("InventoryItems", string.Empty);
        if (!string.IsNullOrEmpty(savedItems))
        {
            string[] items = savedItems.Split(';');
            for (int i = 0; i < items.Length; i++)
            {
                if (i < inventorySlots.Count)
                {
                    string[] itemData = items[i].Split(',');
                    string itemName = itemData[0];
                    int itemTier = int.Parse(itemData[1]);
                    DroppedEquipment.EquipmentType itemType = (DroppedEquipment.EquipmentType)System.Enum.Parse(typeof(DroppedEquipment.EquipmentType), itemData[2]);

                    GameObject itemPrefab = Resources.Load<GameObject>(itemName);
                    if (itemPrefab != null)
                    {
                        Image slotImage = inventorySlots[i].GetComponent<Image>();
                        slotImage.sprite = itemPrefab.GetComponent<SpriteRenderer>().sprite;

                        InventorySlot inventorySlot = inventorySlots[i].GetComponent<InventorySlot>();
                        inventorySlot.itemName = itemName;
                        inventorySlot.itemTier = itemTier;
                        inventorySlot.itemType = itemType;
                    }
                }
            }
        }
    }
}

public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public string itemName;
    public int itemTier;
    public DroppedEquipment.EquipmentType itemType;

    private Transform originalParent;
    private Image slotImage;
    private CanvasGroup canvasGroup;
    private InventorySystem inventorySystem;

    void Start()
    {
        slotImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        inventorySystem = FindObjectOfType<InventorySystem>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(inventorySystem.transform); // Move slot to be a child of the inventory system UI to allow dragging
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Follow the mouse position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent); // Return slot to original parent
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

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
            string newItemName = itemType.ToString() + "T" + newTier;

            // Find and load the new item prefab
            GameObject newItemPrefab = Resources.Load<GameObject>(newItemName);
            if (newItemPrefab != null)
            {
                slotImage.sprite = newItemPrefab.GetComponent<SpriteRenderer>().sprite; // Set the new item sprite
                itemName = newItemName;
                itemTier = newTier;

                Destroy(otherSlot.gameObject); // Remove the dragged item slot

                // Update and save the inventory
                inventorySystem.SaveInventory();
            }
        }
    }
}
