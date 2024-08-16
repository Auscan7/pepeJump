using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 20f; // Damage the player deals on collision

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        DroppedEquipment droppedEquipment = other.GetComponent<DroppedEquipment>();

        if (droppedEquipment != null)
        {
            CollectEquipment(droppedEquipment);
            Destroy(other.gameObject);
        }
    }

    void CollectEquipment(DroppedEquipment droppedEquipment)
    {
        // Here you can add the equipment to the player's inventory (to be implemented later)
        Debug.Log("Collected " + droppedEquipment.equipmentName + " (Tier " + droppedEquipment.equipmentTier + ")");
    }
}
