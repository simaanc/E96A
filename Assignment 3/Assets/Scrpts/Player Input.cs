using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool jumpRequested = false; // To track if jump is requested
    private bool canJump = true; // To control when the player can jump again
    private int jumpCount = 0; // To keep track of the number of jumps

    private Animator animator; // This is initialized in Start and should be used directly

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private int maxJump = 2; // Set to 2 for double jump

    private Vector2 moveInput = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        Move();

        // Jump if jump was requested and we have not jumped too many times
        if (jumpRequested && canJump && (isGrounded || jumpCount < maxJump))
        {
            Jump();
            jumpRequested = false; // Reset jump request
            canJump = false; // Prevent new jump until key is released
        }

        UpdateAnimationStates();
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // Reset the y velocity before applying the jump force to ensure consistent jump height
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        jumpCount++;
        isGrounded = false; // Set isGrounded to false when a jump is made
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && (isGrounded || jumpCount < maxJump))
        {
            jumpRequested = true;
        }
        else if (!value.isPressed)
        {
            canJump = true; // Allow jumping again once the jump key is released
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckIfGrounded(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CheckIfGrounded(collision);
    }

    private void CheckIfGrounded(Collision2D collision)
    {
        isGrounded = false; // Assume not grounded until proven otherwise

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            if (normal.y > 0.2f) // Adjust the 0.5f value as needed
            {
                isGrounded = true;
                jumpCount = 0; // Reset the jump count when grounded
                break; // Exit the loop if grounded
            }
        }
    }

    private void UpdateAnimationStates()
    {
        if (rb.velocity.y > 0.1)
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsFalling", false);
        }
        else if (rb.velocity.y < -0.1)
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
        }

        bool isRunning = Mathf.Abs(moveInput.x) > 0.1f;
        animator.SetBool("IsRunning", isRunning);
    }
}