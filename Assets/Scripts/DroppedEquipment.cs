using UnityEngine;
using System.Collections;

public class DroppedEquipment : MonoBehaviour
{
    public string equipmentName;  // Name of the equipment
    public int equipmentTier;     // Tier of the equipment
    public float collectionDelay = 2f; // Delay before the item can be collected
    public float upwardForce = 5f; // Force applied upwards to the dropped item
    public float shrinkSpeed = 2f; // Speed at which the item shrinks
    public float moveSpeed = 5f;   // Speed at which the item moves towards the player

    private bool isCollected = false; // Flag to track if the item has been collected
    private Rigidbody2D rb;           // Rigidbody2D component of the item
    private Collider2D coll;          // Collider2D component of the item

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        if (rb != null)
        {
            // Apply an upward force to the dropped equipment
            rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
        }

        if (coll != null)
        {
            // Initially disable the collider to prevent immediate collection
            coll.enabled = false;
        }

        // Start the coroutine to handle the delay before collection
        StartCoroutine(HandleCollectionDelay());
    }

    IEnumerator HandleCollectionDelay()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(collectionDelay);

        // Check if the item has not been collected yet
        if (!isCollected && coll != null)
        {
            // Enable the collider to allow collection
            coll.enabled = true;

            // Start the collection animation
            StartCoroutine(MoveAndShrinkTowardsPlayer());
        }
    }

    IEnumerator MoveAndShrinkTowardsPlayer()
    {
        // Disable the collider to prevent further interaction
        if (coll != null)
        {
            coll.enabled = false;
        }

        // Disable gravity to prevent falling
        if (rb != null)
        {
            rb.gravityScale = 0f; // Disable gravity
        }

        // Find the player's position (you may need to adjust this based on your game setup)
        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
            yield break;
        }

        // Move and shrink the item towards the player
        float startTime = Time.time;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 playerPosition = player.transform.position;
        float duration = 1f; // Duration of the move and shrink animation

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

        // Destroy the dropped equipment object after collection
        Destroy(gameObject);
    }
}
