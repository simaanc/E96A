using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Rigidbody rb;
    private bool isGrounded = true;
    private int jumpCount = 0; // Added to track the number of jumps

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpHeight = 5.0f;
    [SerializeField] private int maxJump = 1; // Maximum number of jumps (1 for normal jump, +1 for double jump)

    private Vector2 direction = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        Debug.Log(direction); // (0.0, 0.0) when no key is pressed

        this.direction = direction;
    }

    void Update()
    {
        Move(direction.x, direction.y);
    }

    private void Move(float x, float z)
    {
        rb.velocity = new Vector3(x * speed, rb.velocity.y, z * speed);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }

    void OnJump()
    {
        if (isGrounded || jumpCount < maxJump)
        {
            Jump();
            jumpCount++;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (Vector3.Angle(collision.GetContact(0).normal, Vector3.up) < 45f)
        {
            isGrounded = true;
            jumpCount = 0; // Reset jump count when grounded
        }
        else isGrounded = false;
    }
}