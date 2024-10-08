using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100f;
    public float damage = 20f; // Damage the player deals on collision
    public float armor = 5f; // Armor value (mitigates damage taken)

    public float pushBackForce = 10f;

    public AudioClip attackSound; // Assign your attack sound effect in the Inspector
    public ParticleSystem attackParticle; // Assign your attack particle effect in the Inspector
    private AudioSource audioSource; // Reference to the AudioSource component

    public Image healthBarFill;  // Reference to the health bar fill image

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
        // Calculate the effective damage after armor mitigation
        float effectiveDamage = Mathf.Max(0f, amount - armor);

        currentHealth -= effectiveDamage;
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    public void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Player Died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

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
                //attackParticle.transform.position = collision.contacts[0].point;
                //attackParticle.Play();

                // Start the attack state
                GetComponent<PlayerMovement>().StartAttack();

                // Apply a force to the player towards the opposite side of the enemy

                //Rigidbody2D playerRigidbody = GetComponent<Rigidbody2D>();
                //Vector2 forceDirection = (transform.position + enemy.transform.position).normalized;
                //playerRigidbody.AddForce(-forceDirection * pushBackForce, ForceMode2D.Impulse);

                // Apply a force to the player away from the enemy
                Rigidbody2D playerRigidbody = GetComponent<Rigidbody2D>();
                Vector2 forceDirection = (enemy.transform.position - transform.position).normalized;
                playerRigidbody.AddForce(-forceDirection * pushBackForce, ForceMode2D.Impulse);

            }
        }
    }
}