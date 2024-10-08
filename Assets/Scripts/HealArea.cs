using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Heal(collision);
            Destroy(gameObject);
        }
    }

    private void Heal(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        player.currentHealth = player.maxHealth;
        player.UpdateHealthBar();
    }
}
