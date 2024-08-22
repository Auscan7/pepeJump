using UnityEngine;
using System.Collections;

public class DroppedEquipment : MonoBehaviour
{
    public string equipmentName;
    public int equipmentTier;
    public float initialForce = 5f;
    public float upwardForce = 2f;
    public float collectionDelay = 1f;
    public EquipmentType equipmentType;
    public Sprite inventorySprite;

    public enum EquipmentType { Weapon, Armor, Gloves, Boots }

    private Rigidbody2D rb;
    private Collider2D coll;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        if (coll != null)
        {
            coll.enabled = false;
        }

        if (rb != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                Vector2 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
                Vector2 force = directionAwayFromPlayer * initialForce + Vector2.up * upwardForce;
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }

        StartCoroutine(EnableColliderAfterDelay());
    }

    IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(collectionDelay);

        if (coll != null)
        {
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
                string itemName = gameObject.name.Replace("(Clone)", "").Trim();
                bool itemAdded = player.inventorySystem.AddItem(itemName, equipmentTier, equipmentType);

                if (itemAdded)
                {
                    StartCoroutine(MoveAndShrinkTowardsPlayer(player));
                }
                else
                {
                    Debug.Log("Inventory is full, item not collected.");
                    if (coll != null)
                    {
                        coll.enabled = true;
                    }
                    if (rb != null)
                    {
                        rb.gravityScale = 1f;
                    }
                }
            }
        }
    }

    IEnumerator MoveAndShrinkTowardsPlayer(Player player)
    {
        if (coll != null)
        {
            coll.enabled = false;
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
        }

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 playerPosition = player.transform.position;
        float duration = 0.3f;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            transform.position = Vector3.Lerp(transform.position, playerPosition, t);

            yield return null;
        }

        transform.localScale = endScale;
        transform.position = playerPosition;

        // Destroy the item after the collection animation
        Destroy(gameObject);
    }
}
