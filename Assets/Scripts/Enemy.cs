using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;
    public int moneyReward = 50;  // Amount of money earned when this enemy is killed
    public Image healthBarFill;  // Reference to the health bar fill image
    public float itemDropChance = 40f; // Public variable to control item drop chance
    public List<Drop> possibleDrops = new List<Drop>();
    public SpriteRenderer spriteRenderer; // Reference to the enemy's sprite renderer

    private float currentHealth;
    private Color originalColor; // Store the original color of the sprite

    // Delegate and event for death notification
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    [System.Serializable]
    public class EquipmentDrop
    {
        public string prefabName;  // Name of the prefab in the Resources folder
        public float dropChance;   // Chance of dropping this equipment
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Modified to use GetComponentInChildren
        originalColor = spriteRenderer.color; // Store the original color
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();

        // Flash the enemy's sprite when hit
        StartCoroutine(FlashSprite());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    IEnumerator FlashSprite()
    {
        spriteRenderer.color = Color.red; // or any other color that will stand out
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        // Trigger the OnDeath event
        OnDeath?.Invoke();

        // Award money to the player with the money sprite animation
        MoneyManager.Instance.AddMoney(moneyReward, transform.position);

        // Drop equipment if applicable
        DropEquipment();

        // Destroy the enemy object
        Destroy(gameObject);
    }

    void DropEquipment()
    {
        // Roll a random value to determine if an item should be dropped
        float roll = Random.Range(0f, 100f); // Roll a value between 0 and 100

        // Check if the roll is less than the item drop chance
        if (roll <= itemDropChance)
        {
            // Calculate total drop chance for the items
            float totalItemChance = 0f;
            foreach (var drop in possibleDrops)
            {
                totalItemChance += drop.dropChance;
            }

            // Normalize the item drop chances
            foreach (var drop in possibleDrops)
            {
                drop.dropChance /= totalItemChance;
                drop.dropChance *= 100f; // Convert to percentage
            }

            // Roll a random value to determine which item to drop
            float itemRoll = Random.Range(0f, 100f); // Roll a value between 0 and 100

            // Select one item to drop based on its chance
            float cumulativeChance = 0f;

            foreach (var drop in possibleDrops)
            {
                cumulativeChance += drop.dropChance;
                if (itemRoll <= cumulativeChance)
                {
                    // Load the appropriate prefab from Resources
                    GameObject droppedEquipmentGO = Instantiate(Resources.Load(drop.prefabName), transform.position, Quaternion.identity) as GameObject;

                    if (droppedEquipmentGO == null)
                    {
                        Debug.LogWarning($"Prefab '{drop.prefabName}' not found in Resources.");
                        return; // Exit if prefab not found
                    }

                    // Optionally, ensure the dropped equipment has a DroppedEquipment component
                    // and configure it as necessary
                    DroppedEquipment droppedEquipment = droppedEquipmentGO.GetComponent<DroppedEquipment>();
                    if (droppedEquipment != null)
                    {
                        // Configure dropped equipment if necessary
                    }

                    break; // Exit loop after dropping one item
                }
            }
        }
    }

    [System.Serializable]
    public class Drop
    {
        public string prefabName;
        public float dropChance;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}