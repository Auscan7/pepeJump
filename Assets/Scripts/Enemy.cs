using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;
    public int moneyReward = 50;  // Amount of money earned when this enemy is killed
    public Image healthBarFill;  // Reference to the health bar fill image
    public EquipmentDrop[] possibleDrops; // List of possible equipment drops

    private float currentHealth;

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

        if (currentHealth <= 0f)
        {
            Die();
        }
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
        // Calculate total drop chance
        float totalDropChance = 0f;
        foreach (var drop in possibleDrops)
        {
            totalDropChance += drop.dropChance;
        }

        // Select one item to drop based on its chance
        float roll = Random.Range(0f, totalDropChance);
        float cumulativeChance = 0f;

        foreach (var drop in possibleDrops)
        {
            cumulativeChance += drop.dropChance;
            if (roll <= cumulativeChance)
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
