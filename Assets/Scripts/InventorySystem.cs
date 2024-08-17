using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the Inventory UI
    public GameObject slotPrefab;  // Reference to the Inventory Slot prefab
    public int inventorySize = 20; // Total number of slots in the inventory

    private List<GameObject> inventorySlots = new List<GameObject>(); // List to hold all slots

    void Start()
    {
        // Initialize the inventory UI
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);
            inventorySlots.Add(slot);
        }

        // Initially hide the inventory UI
        inventoryUI.SetActive(false);
    }

    public bool AddItem(Sprite itemIcon)
    {
        // Find the first empty slot
        foreach (GameObject slot in inventorySlots)
        {
            Image slotImage = slot.GetComponent<Image>();

            if (slotImage.sprite == null)
            {
                slotImage.sprite = itemIcon; // Set the item icon in the empty slot
                return true; // Successfully added the item
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
    }
}
