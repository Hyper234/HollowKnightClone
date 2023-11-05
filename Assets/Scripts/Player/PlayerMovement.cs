using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    [SerializeField] private float wallSlidingSpeed;

    [Header("Dash")]
    [SerializeField] private float dashingSpeed;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashCooldown;

    [Header("Collisions")]
    [SerializeField] private LayerMask groundLayerMask;

    private bool isOnWall = false;
    private bool isGrounded = false;
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
        //ground check
        if (IsGrounded())
        {
            canDash = true;
            doubleJumpAvailable = true;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //wall check
        if (IsOnWall())
        {
            isOnWall = true;
            doubleJumpAvailable = true;
        }
        else
        {
            isOnWall = false;
        }

        //movement
        if (!isDashing)
        {
            Move();
        }

        //animations
        animator.SetBool("running", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded);

        //update timers
        dashTimer += Time.deltaTime;
    }

    private void Move()
    {
        //read player input
        horizontalInput = playerInputActions.Player.Movement.ReadValue<float>();

        if (isOnWall)
        {
            if(rigidBody2D.velocity.y > 0)
                //if the player is jumping against the wall don't wall slide
                rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, rigidBody2D.velocity.y);
            else
                //if the player is falling against the wall then wall slide
                rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, -wallSlidingSpeed);
        }
        else
        {
            //default movement
            rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, rigidBody2D.velocity.y);
        }

        FlipSprite();
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        //the player cannot jump while dashing
        if (!isDashing)
        {
            if (isGrounded)
            {
                if (jumpHeld)
                {
                    //default jump
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                    animator.SetTrigger("jump");
                }
            }
            else
            {
                if (jumpHeld)
                {
                    if (doubleJumpAvailable)
                    {
                        //double jump if player not grounded
                        rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                        animator.SetTrigger("jump");
                        doubleJumpAvailable = false;
                    }
                }
                else
                {
                    //resets player's vertical movement speed when jump released
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Min(0, rigidBody2D.velocity.y));
                }
            }
        }

        jumpHeld = !jumpHeld;
    }

    private void Dash_performed(InputAction.CallbackContext obj)
    {
        //checking if the player can dash
        if (!isDashing && canDash && dashTimer > dashCooldown)
        {
            //perform dash
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        //set all states and dashing animation
        canDash = false;
        isDashing = true;
        animator.SetBool("dashing", isDashing);
        animator.SetTrigger("dash_init");

        //set dashing movement
        float gravity = rigidBody2D.gravityScale;
        rigidBody2D.gravityScale = 0;
        rigidBody2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashingSpeed, 0);

        //wait for dash to end
        yield return new WaitForSeconds(dashingTime);

        //reset all movement after the dash
        rigidBody2D.gravityScale = gravity;
        rigidBody2D.velocity = Vector2.zero;

        //stop dashing animation and start dash cooldown timer
        isDashing = false;
        animator.SetBool("dashing", isDashing);
        dashTimer = 0f;
    }

    private void FlipSprite()
    {
        //flip the sprite depending on the horizontal velocity
        if (rigidBody2D.velocity.x < 0)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }

    //checking if the player is touching the ground
    private bool IsGrounded()
    {
        float margin = .02f;

        return Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, margin, groundLayerMask).collider != null;
    }

    //checking if the player is touching a wall
    private bool IsOnWall()
    {
        float margin = .02f;

        bool left = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.left, margin, groundLayerMask).collider != null;
        bool right = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.right, margin, groundLayerMask).collider != null;

        return left || right;
    }
}
