using System.Collections;
using System.Collections.Generic;
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
        public Equipment equipment;
        public float dropChance; // Chance of dropping this equipment
        public float[] tierChances; // Chances for each tier
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

        // Award money to the player
        MoneyManager.Instance.AddMoney(moneyReward);

        // Drop equipment if applicable
        DropEquipment();

        // Destroy the enemy object
        Destroy(gameObject);
    }

    void DropEquipment()
    {
        foreach (var drop in possibleDrops)
        {
            float dropRoll = Random.Range(0f, 100f);
            if (dropRoll <= drop.dropChance)
            {
                // Determine the tier of the dropped equipment
                int tier = DetermineTier(drop.tierChances);
                if (tier != -1)
                {
                    Equipment droppedEquipment = Instantiate(drop.equipment);
                    droppedEquipment.tier = tier;
                    Instantiate(droppedEquipment, transform.position, Quaternion.identity);
                }
            }
        }
    }

    int DetermineTier(float[] tierChances)
    {
        float cumulativeChance = 0f;
        float roll = Random.Range(0f, 100f);

        for (int i = 0; i < tierChances.Length; i++)
        {
            cumulativeChance += tierChances[i];
            if (roll <= cumulativeChance)
            {
                return i + 1; // Tier is 1-based, so add 1
            }
        }

        return -1; // No tier determined
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

