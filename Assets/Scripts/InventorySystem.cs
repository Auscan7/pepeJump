using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryUI;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int inventorySize = 20;

    public Sprite placeholderSprite;

    private List<GameObject> inventorySlots = new List<GameObject>();
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();
    public GridLayoutGroup gridLayoutGroup; // Reference to GridLayoutGroup

    void Start()
    {
        gridLayoutGroup = inventoryPanel.GetComponent<GridLayoutGroup>(); // Initialize GridLayoutGroup

        // Initialize the inventory UI
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);
            slot.AddComponent<InventorySlot>();
            inventorySlots.Add(slot);
        }

        // Initially hide the inventory UI
        inventoryUI.SetActive(false);

        // Load inventory items
        LoadInventory();
    }

    public bool AddItem(string itemName, int tier = 1, DroppedEquipment.EquipmentType type = DroppedEquipment.EquipmentType.Weapon)
    {
        foreach (GameObject slot in inventorySlots)
        {
            if (slot == null) continue;

            Image slotImage = slot.GetComponent<Image>();
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (slotImage == null || inventorySlot == null) continue;

            if (slotImage.sprite == null || slotImage.sprite == placeholderSprite)
            {
                GameObject itemPrefab = Resources.Load<GameObject>(itemName);
                if (itemPrefab != null)
                {
                    DroppedEquipment droppedEquipment = itemPrefab.GetComponent<DroppedEquipment>();
                    if (droppedEquipment != null)
                    {
                        slotImage.sprite = droppedEquipment.inventorySprite;

                        inventorySlot.itemName = itemName;
                        inventorySlot.itemTier = tier;
                        inventorySlot.itemType = type;

                        inventoryItems.Add(new InventoryItem
                        {
                            name = itemName,
                            tier = tier,
                            type = type,
                            spriteName = droppedEquipment.inventorySprite.name
                        });

                        SaveInventory();
                        return true;
                    }
                }
            }
        }

        Debug.Log("Inventory is full!");
        return false;
    }

    public void UpdateInventorySlot(InventorySlot updatedSlot)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySlots[i].GetComponent<InventorySlot>();
            if (slot != null && slot.itemName == updatedSlot.itemName && slot.itemTier == updatedSlot.itemTier && slot.itemType == updatedSlot.itemType)
            {
                slot.itemName = updatedSlot.itemName;
                slot.itemTier = updatedSlot.itemTier;
                slot.itemType = updatedSlot.itemType;
                slot.GetComponent<Image>().sprite = updatedSlot.GetComponent<Image>().sprite;
                break;
            }
        }
    }

    public void ToggleInventory()
    {
        // Toggle the inventory UI on or off
        inventoryUI.SetActive(!inventoryUI.activeSelf);

        // Ensure no UI button stays selected after clicking
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SaveInventory()
    {
        List<string> savedItems = new List<string>();
        foreach (var item in inventoryItems)
        {
            savedItems.Add(item.name + "," + item.tier + "," + item.type + "," + item.spriteName);
        }

        Debug.Log("Saving Inventory: " + string.Join(";", savedItems));

        PlayerPrefs.SetString("InventoryItems", string.Join(";", savedItems));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        inventoryItems.Clear(); // Clear existing inventory items

        // Clear all slots
        foreach (GameObject slot in inventorySlots)
        {
            Image slotImage = slot.GetComponent<Image>();
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (slotImage != null && inventorySlot != null)
            {
                slotImage.sprite = placeholderSprite;
                inventorySlot.itemName = string.Empty;
                inventorySlot.itemTier = 0;
                inventorySlot.itemType = DroppedEquipment.EquipmentType.None;
            }
        }

        string savedItems = PlayerPrefs.GetString("InventoryItems", string.Empty);
        if (!string.IsNullOrEmpty(savedItems))
        {
            string[] items = savedItems.Split(';');
            Debug.Log("Loading Inventory: " + savedItems);
            foreach (var item in items)
            {
                string[] itemData = item.Split(',');
                if (itemData.Length == 4)
                {
                    string itemName = itemData[0];
                    int itemTier = int.Parse(itemData[1]);
                    DroppedEquipment.EquipmentType itemType = (DroppedEquipment.EquipmentType)System.Enum.Parse(typeof(DroppedEquipment.EquipmentType), itemData[2]);
                    string spriteName = itemData[3];

                    Sprite itemSprite = Resources.Load<Sprite>(spriteName);
                    if (itemSprite != null)
                    {
                        bool itemAdded = false;
                        for (int i = 0; i < inventorySlots.Count; i++)
                        {
                            Image slotImage = inventorySlots[i].GetComponent<Image>();
                            InventorySlot inventorySlot = inventorySlots[i].GetComponent<InventorySlot>();

                            if (slotImage != null && (slotImage.sprite == null || slotImage.sprite == placeholderSprite))
                            {
                                slotImage.sprite = itemSprite;
                                inventorySlot.itemName = itemName;
                                inventorySlot.itemTier = itemTier;
                                inventorySlot.itemType = itemType;

                                inventoryItems.Add(new InventoryItem
                                {
                                    name = itemName,
                                    tier = itemTier,
                                    type = itemType,
                                    spriteName = spriteName
                                });

                                itemAdded = true;
                                break;
                            }
                        }

                        if (!itemAdded)
                        {
                            Debug.LogWarning("No empty slot found to load item: " + itemName);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Sprite with name '{spriteName}' not found.");
                    }
                }
            }
        }
        else
        {
            Debug.Log("No saved inventory data found.");
        }
    }

    [System.Serializable]
    public class InventoryItem
    {
        public string name;
        public int tier;
        public DroppedEquipment.EquipmentType type;
        public string spriteName;
    }
}
