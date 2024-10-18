using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float boostModifier = 2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] int maxJumps = 2;

    [SerializeField] Animator animator;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 inputDirection = Vector2.zero;
    private int numJumps = 0;
    private bool isGrounded = true;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 1f;
    }

    void OnMoving(InputValue value)
    {
        // Get the movement direction from input
        Vector2 movementDir = value.Get<Vector2>();
        inputDirection = movementDir;

        // Update the running animation only if there is horizontal movement
        bool isMovingHorizontally = Mathf.Abs(inputDirection.x) > 0.1f;
        animator.SetBool("running", isMovingHorizontally);

        // Flip the sprite based on direction
        if (movementDir.x < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
        else if (movementDir.x > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
    }

    void OnJump(InputValue value)
    {
        Debug.Log("Jump action triggered");
        // Check if grounded or if the vertical speed is nearly zero
        if (isGrounded || numJumps < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Reset Y velocity
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply jump force
            numJumps++;
            isGrounded = false; // Player is airborne after jump
            animator.SetBool("isJumping", true);
            Debug.Log("Jumped. Current numJumps: " + numJumps);
        }
        else
        {
            Debug.Log("Max jumps reached or not grounded.");
        }
    }

    private void Update()
    {
        // Apply horizontal movement based on input
        if (Mathf.Abs(inputDirection.x) > 0.1f)
        {
            rb.velocity = new Vector2(inputDirection.x * boostModifier, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Stop horizontal movement when no input
            animator.SetBool("running", false); // Ensure running animation stops
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is grounded
            numJumps = 0; // Reset jump count on landing
            animator.SetBool("isJumping", false); // Set isJumping to false when grounded
            Debug.Log("Landed on ground. numJumps reset to 0.");
        }
        Debug.Log("Collision detected with: " + collision.gameObject.name);
    }
}
