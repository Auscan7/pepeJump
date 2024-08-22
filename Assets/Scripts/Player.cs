using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 20f; // Damage the player deals on collision
    public InventorySystem inventorySystem; // Reference to the InventorySystem

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        inventorySystem = FindObjectOfType<InventorySystem>(); // Find the inventory system in the scene
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        // Add your death logic here (e.g., restart the level)
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void CollectEquipment(DroppedEquipment droppedEquipment)
    {
        // This method is now handled by OnTriggerEnter2D, but we'll keep it for additional use if needed
        Debug.Log("Collected " + droppedEquipment.equipmentName + " (Tier " + droppedEquipment.equipmentTier + ")");
    }
}
