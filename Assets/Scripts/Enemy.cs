using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;
    public int moneyReward = 50;  // Amount of money earned when this enemy is killed
    public Image healthBarFill;  // Reference to the health bar fill image
    public SpriteRenderer spriteRenderer; // Reference to the enemy's sprite renderer

    private float currentHealth;
    private Color originalColor; // Store the original color of the sprite

    // Delegate and event for death notification
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

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

        // Destroy the enemy object
        Destroy(gameObject);
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