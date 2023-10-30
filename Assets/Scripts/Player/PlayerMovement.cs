using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private BoxCollider2D boxCollider2D;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpPower;

    [Header("Dash")]
    [SerializeField] private float dashingSpeed;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashCooldown;

    [Header("Collisions")]
    [SerializeField] private LayerMask platformLayerMask;

    private float horizontalInput;
    private bool jumpHeld = true;
    private bool doubleJumpAvailable = true;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimer = 1000f;


    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Dash.performed += Dash_performed;
    }

    private void Update()
    {
        if (!isDashing)
        {
            Move();
            FlipSprite();
        }

        if (IsGrounded())
        {
            canDash = true;
        }

        animator.SetBool("running", horizontalInput != 0);
        animator.SetBool("grounded", IsGrounded());

        dashTimer += Time.deltaTime;
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
        {
            if (jumpHeld)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                animator.SetTrigger("jump");
            }

            doubleJumpAvailable = true;
        }
        else
        {
            if (jumpHeld)
            {
                if (doubleJumpAvailable)
                {
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                    animator.SetTrigger("jump");
                    doubleJumpAvailable = false;
                }
            }
            else
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Min(0, rigidBody2D.velocity.y));
            }
        }

        jumpHeld = !jumpHeld;
    }

    private void Move()
    {
        horizontalInput = playerInputActions.Player.Movement.ReadValue<float>();
        rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, rigidBody2D.velocity.y);
    }

    private void Dash_performed(InputAction.CallbackContext obj)
    {
        if (!isDashing && canDash && dashTimer > dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float gravity = rigidBody2D.gravityScale;
        rigidBody2D.gravityScale = 0;
        rigidBody2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashingSpeed, 0);

        yield return new WaitForSeconds(dashingTime);

        rigidBody2D.gravityScale = gravity;
        rigidBody2D.velocity = Vector2.zero;

        isDashing = false;
        dashTimer = 0f;
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
