using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float minJumpForce = 5f;
    public float maxJumpForce = 20f;
    public float maxHoldTime = 2f; // Time in seconds to reach maximum jump force
    public float moveSpeed = 5f; // Speed for horizontal jump movement
    public float groundCheckRadius = 0.1f; // Radius for ground detection
    public LayerMask groundLayer; // LayerMask to define what is ground

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isHoldingJump = false;
    private float holdTime = 0f;
    private float moveDirection = 0f;

    public Transform groundCheck; // Empty GameObject positioned at the player's feet

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
    }

    void HandleInput()
    {
        // Capture direction input while holding jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isHoldingJump = true;
            holdTime = 0f;

            // Check for direction input
            moveDirection = Input.GetAxisRaw("Horizontal"); // -1 for left, 1 for right, 0 for no input
        }

        // While holding the jump
        if (isHoldingJump && Input.GetButton("Jump"))
        {
            holdTime += Time.deltaTime;
        }

        // Release jump button
        if (isHoldingJump && Input.GetButtonUp("Jump"))
        {
            Jump();
            isHoldingJump = false;
        }
    }

    void Jump()
    {
        // Calculate jump force based on hold time
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, holdTime / maxHoldTime);
        float horizontalJump = moveDirection * moveSpeed;

        // Apply jump force in the direction
        rb.velocity = new Vector2(horizontalJump, jumpForce);
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
}
