using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Heal(collision);
        }
    }

    private void Heal(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        player.currentHealth = player.maxHealth;
        player.UpdateHealthBar();
    }
}
