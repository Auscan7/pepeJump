using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;
    public Image healthBarFill;  // Reference to the health bar fill image

    private float currentHealth;

    // Delegate and event for death notification
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

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

        // Destroy the enemy object
        Destroy(gameObject);
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

