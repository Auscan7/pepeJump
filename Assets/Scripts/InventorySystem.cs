using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the Panel with the GridLayoutGroup
    public GameObject inventroyPanel; //To add slot prefabs as a child
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
            GameObject slot = Instantiate(slotPrefab, inventroyPanel.transform);
            inventorySlots.Add(slot);
        }

        // Initially hide the inventory UI
        inventoryUI.SetActive(false);

        // Load inventory items
        LoadInventory();
    }

    public bool AddItem(string itemName)
    {


        // Find the first empty or placeholder slot
        foreach (GameObject slot in inventorySlots)
        {
            Image slotImage = slot.GetComponent<Image>();

            if (slotImage.sprite == null || slotImage.sprite == placeholderSprite)
            {
                GameObject itemPrefab = Resources.Load<GameObject>(itemName);
                if (itemPrefab != null)
                {
                    slotImage.sprite = itemPrefab.GetComponent<SpriteRenderer>().sprite; // Set the item sprite
                    inventoryItems.Add(itemName); // Add item name to the list for saving
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


    private void SaveInventory()
    {
        PlayerPrefs.SetString("InventoryItems", string.Join(",", inventoryItems));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        string savedItems = PlayerPrefs.GetString("InventoryItems", string.Empty);
        if (!string.IsNullOrEmpty(savedItems))
        {
            inventoryItems = savedItems.Split(',').ToList();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (i < inventorySlots.Count)
                {
                    GameObject itemPrefab = Resources.Load<GameObject>(inventoryItems[i]);
                    if (itemPrefab != null)
                    {
                        Image slotImage = inventorySlots[i].GetComponent<Image>();
                        slotImage.sprite = itemPrefab.GetComponent<SpriteRenderer>().sprite;
                    }
                }
            }
        }
    }
}
