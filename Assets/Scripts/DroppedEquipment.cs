using UnityEngine;
using System.Collections;

public class DroppedEquipment : MonoBehaviour
{
    public string equipmentName;  // Name of the equipment
    public int equipmentTier;     // Tier of the equipment
    public float initialForce = 5f; // Force applied in the opposite direction of the player
    public float upwardForce = 2f;  // Upward force applied to the dropped item
    public float collectionDelay = 1f; // Delay before the item can be collected

    private Rigidbody2D rb; // Rigidbody2D component of the item
    private Collider2D coll; // Collider2D component of the item

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        if (coll != null)
        {
            // Disable the collider initially to prevent immediate collection
            coll.enabled = false;
        }

        if (rb != null)
        {
            // Find the player
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Calculate the direction away from the player
                Vector2 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;

                // Apply force in the opposite direction of the player with a slight upward force
                Vector2 force = directionAwayFromPlayer * initialForce + Vector2.up * upwardForce;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        // Start the coroutine to enable the collider after a delay
        StartCoroutine(EnableColliderAfterDelay());
    }

    IEnumerator EnableColliderAfterDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(collectionDelay);

        if (coll != null)
        {
            // Enable the collider to allow collection
            coll.enabled = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                // Start the collection process
                StartCoroutine(MoveAndShrinkTowardsPlayer(player));
            }
        }
    }

    IEnumerator MoveAndShrinkTowardsPlayer(Player player)
    {
        // Disable the collider to prevent further interaction
        if (coll != null)
        {
            coll.enabled = false;
        }

        // Disable gravity to prevent falling during the collection
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero; // Stop any existing motion
        }

        // Move and shrink the item towards the player
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 playerPosition = player.transform.position;
        float duration = 0.3f; // Duration of the move and shrink animation
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            transform.position = Vector3.Lerp(transform.position, playerPosition, t);

            yield return null;
        }

        // Ensure final state is reached
        transform.localScale = endScale;
        transform.position = playerPosition;

        // Collect the item (e.g., add to inventory) and destroy the object
        player.CollectEquipment(this);
        Destroy(gameObject);
    }
}
