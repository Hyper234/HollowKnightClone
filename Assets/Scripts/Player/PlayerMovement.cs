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
    private bool isSliding = false;


    private void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Jump.canceled += Jump_canceled;
        playerInputActions.Player.Dash.performed += Dash_performed;
    }

    private void Update()
    {
        //ground check
        if (IsGrounded())
        {
            isGrounded = true;
            canDash = true;
            doubleJumpAvailable = true;
        }
        else
        {
            isGrounded = false;
        }

        //wall check
        if (IsOnWall())
        {
            isOnWall = true;
            canDash = true;
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
    }

    private void LateUpdate()
    {
        FlipSprite();

        //animations
        animator.SetBool("running", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("sliding", isSliding);
        animator.SetBool("dashing", isDashing);

        //update timers
        dashTimer += Time.deltaTime;
    }

    private void Move()
    {
        //read player input
        horizontalInput = playerInputActions.Player.Movement.ReadValue<float>();

        if (isOnWall)
        {
            if(rigidBody2D.velocity.y >= 0)
            {
                //if the player is jumping against the wall don't wall slide
                rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, rigidBody2D.velocity.y);
                isSliding = false;
            }
            else
            {
                //if the player is falling against the wall then wall slide
                rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, -wallSlidingSpeed);
                isSliding = true;
            }
        }
        else
        {
            //default movement
            rigidBody2D.velocity = new Vector2(horizontalInput * movementSpeed, rigidBody2D.velocity.y);
            isSliding = false;
        }
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        //the player cannot jump while dashing
        if (!isDashing)
        {
            if (isGrounded)
            {
                //default jump
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                animator.SetTrigger("jump");
            }
            else
            {
                if (doubleJumpAvailable)
                {
                    //double jump if player not grounded
                    rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpPower);
                    animator.SetTrigger("jump");
                    doubleJumpAvailable = false;
                }
            }
        }

        jumpHeld = !jumpHeld;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        if(!isDashing && !isGrounded)
        {
            //resets player's vertical movement speed when jump released
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Min(0, rigidBody2D.velocity.y));
        }
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
        animator.SetTrigger("dash_init");

        //set dashing movement
        float gravity = rigidBody2D.gravityScale;
        rigidBody2D.gravityScale = 0;
        if (!isSliding)
        {
            rigidBody2D.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashingSpeed, 0);
        }
        else
        {
            rigidBody2D.velocity = new Vector2(Mathf.Sign(-transform.localScale.x) * dashingSpeed, 0);
            isSliding = false;
        }

        //wait for dash to end
        yield return new WaitForSeconds(dashingTime);

        //reset all movement after the dash
        rigidBody2D.gravityScale = gravity;
        rigidBody2D.velocity = Vector2.zero;

        //stop dashing animation and start dash cooldown timer
        isDashing = false;
        dashTimer = 0f;
    }

    private void FlipSprite()
    {
        //flip the sprite depending on the horizontal velocity
        if (rigidBody2D.velocity.x < 0)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        if (rigidBody2D.velocity.x > 0)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }

    //checking if the player is touching the ground
    private bool IsGrounded()
    {
        float margin = .1f;

        return Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, margin, groundLayerMask).collider != null;
    }

    //checking if the player is touching a wall
    private bool IsOnWall()
    {
        //checking if the player is on the wall with margin to prevent returning true when hitting the ground or the ceiling
        float distance = .02f;
        float margin = .3f;

        Vector2 center = boxCollider2D.bounds.center;
        Vector2 adjustedSize = new Vector2(boxCollider2D.bounds.size.x, boxCollider2D.bounds.size.y - 2 * margin);

        bool left = Physics2D.BoxCast(center, adjustedSize, 0f, Vector2.left, distance, groundLayerMask).collider != null;
        bool right = Physics2D.BoxCast(center, adjustedSize, 0f, Vector2.right, distance, groundLayerMask).collider != null;

        return left || right;
    }
}
