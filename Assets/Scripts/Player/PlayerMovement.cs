using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpPower;

    [Header("Collisions")]
    [SerializeField] private LayerMask platformLayerMask;

    private Rigidbody2D rigidBody2D;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private BoxCollider2D boxCollider2D;
    private Animator animator;

    private float horizontalInput;
    private bool jumpIsPressed = true;

    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_performed;
    }

    private void Update()
    {
        Move();
        FlipSprite();

        animator.SetBool("running", horizontalInput != 0);
        animator.SetBool("grounded", IsGrounded());
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if (IsGrounded() && jumpIsPressed)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
            animator.SetTrigger("jump");
            jumpIsPressed = !jumpIsPressed;
        }
        else if (IsGrounded())
        {
            jumpIsPressed = !jumpIsPressed;
        }
        else
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Math.Min(0, rigidBody2D.velocity.y));
            jumpIsPressed = !jumpIsPressed;
        }
    }

    private void Move()
    {
        horizontalInput = playerInputActions.Player.Movement.ReadValue<float>();
        rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, rigidBody2D.velocity.y);
    }

    private void FlipSprite()
    {
        if (horizontalInput * transform.localScale.x < 0)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private bool IsGrounded()
    {
        float margin = .1f;
        return Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, margin, platformLayerMask).collider != null;
    }
}
