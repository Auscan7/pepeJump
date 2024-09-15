using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10; // adjust the damage amount

    void OnCollisionEnter2D(Collision2D collision)
    {
        // check if the bullet collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // deal damage to the player
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
        }

        // destroy the bullet
        Destroy(gameObject);
    }
}