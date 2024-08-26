using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float maxHoldTime = 2f; // Time in seconds to reach maximum jump force
    public float moveSpeed = 5f; // Speed for horizontal jump movement
    public float groundCheckRadius = 0.1f; // Radius for ground detection
    public LayerMask groundLayer; // LayerMask to define what is ground

    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public Sprite idleSprite; // Sprite for idle state
    public Sprite readyToJumpSprite; // Sprite for getting ready to jump state
    public Sprite jumpSprite; // Sprite for jumping state
    public Sprite fallSprite; // Sprite for falling state

    public AudioClip jumpSound; // Assign your jump sound effect in the Inspector
    public AudioClip landSound; // Assign your land sound effect in the Inspector

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingJump = false;
    private float holdTime = 0f;
    private float moveDirection = 0f;
    private float lastMoveDirection = 0f; // Stores the last horizontal input direction
    private bool wasGrounded = false; // Stores the previous grounded state

    private AudioSource audioSource; // Reference to the AudioSource component

    public Transform groundCheck; // Empty GameObject positioned at the player's feet

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component if it doesn't exist
        }
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
        UpdateSprite();

        // Check if the player has just landed
        if (isGrounded && !wasGrounded)
        {
            // Play land sound effect
            audioSource.PlayOneShot(landSound);
        }

        wasGrounded = isGrounded;
    }

    void HandleInput()
    {
        // Start holding the jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isHoldingJump = true;
            holdTime = 0f;
        }

        // Continuously update the direction based on input
        if (isHoldingJump)
        {
            moveDirection = Input.GetAxisRaw("Horizontal"); // -1 for left, 1 for right, 0 for no input

            // If there is horizontal input, update the lastMoveDirection
            if (moveDirection != 0)
            {
                lastMoveDirection = moveDirection;
            }

            holdTime += Time.deltaTime;
        }

        // Release jump button
        if (isHoldingJump && Input.GetButtonUp("Jump"))
        {
            Jump();
            isHoldingJump = false;
            lastMoveDirection = 0f; // Reset the last remembered horizontal input after the jump
        }
    }

    void Jump()
    {
        // Calculate jump force based on hold time
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, holdTime / maxHoldTime);
        float horizontalJump = lastMoveDirection * moveSpeed;

        // Apply jump force in the direction
        rb.velocity = new Vector2(horizontalJump, jumpForce);

        // Play jump sound effect
        audioSource.PlayOneShot(jumpSound);
    }

    void CheckGrounded()
    {
        // Check if the player is grounded using a small circle at the groundCheck position
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void FixedUpdate()
    {
        // Ensure the player doesn't fall too fast
        if (rb.velocity.y < -20f) // Replace with your max fall speed if different
        {
            rb.velocity = new Vector2(rb.velocity.x, -20f);
        }
    }

    void UpdateSprite()
    {
        // Flip the sprite based on the last move direction
        if (lastMoveDirection != 0)
        {
            spriteRenderer.flipX = lastMoveDirection < 0; // Flip if moving left
        }

        if (isGrounded && !isHoldingJump)
        {
            spriteRenderer.sprite = idleSprite; // Idle state when grounded and not holding jump
        }
        else if (isGrounded && isHoldingJump)
        {
            spriteRenderer.sprite = readyToJumpSprite; // Getting ready to jump state
        }
        else if (rb.velocity.y > 0)
        {
            spriteRenderer.sprite = jumpSprite; // Jumping state when moving upwards
        }
        else if (rb.velocity.y < 0)
        {
            spriteRenderer.sprite = fallSprite; //
        }
    }
}