using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 20f; // Damage the player deals on collision
    public AudioClip attackSound; // Assign your attack sound effect in the Inspector
    public ParticleSystem attackParticle; // Assign your attack particle effect in the Inspector
    private AudioSource audioSource; // Reference to the AudioSource component

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component if it doesn't exist
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        // Add your death logic here (e.g., restart the level)
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                // Play the attack sound effect
                audioSource.PlayOneShot(attackSound);
                // Play the attack particle effect
                attackParticle.transform.position = collision.contacts[0].point;
                attackParticle.Play();

                // Start the attack state
                GetComponent<PlayerMovement>().StartAttack();
            }
        }
    }
}