using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public PlayerMovementStats MoveStats;
    [SerializeField] private Collider2D feetCollider;
    [SerializeField] private Collider2D bodyCollider;

    private Rigidbody2D rb;

    //Movement variables
    private Vector2 moveVelocity;
    private bool isFacingRight;

    //Collision check variables
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    public bool isGrounded;
    private bool bumpedHead;

    //jump variables
    public float VerticalVelocity { get; private set; }
    private bool isJumping;
    private bool isFastFalling; //is true when the player releases jump before they reach the apex of their jump
    private bool isFalling;
    private float fastFallTime; //time it takes the player to go from moving upwards to moving downwards
    private float fastFallReleaseSpeed; //vertical velocity of the player when entering fast fall
    public int numOfJumpsUsed;

    //jump apex variables
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    //jump buffer variables
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    //coyote time variables
    public float coyoteTimer;

    private void Awake()
    {
        isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        if (isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);

            Vector2 targetVelocity = Vector2.zero;
            if (InputManager.RunIsHeld)
            {
                targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxRunSpeed;
            }
            else { targetVelocity = new Vector2(moveInput.x, 0f) * MoveStats.MaxWalkSpeed; }

            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(moveVelocity.x, rb.velocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0, 180, 0);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0, -180, 0);
        }
    }

    #endregion

    #region Jump

    private void JumpChecks()
    {
        //When we press jump
        if (InputManager.JumpWasPressed)
        {
            jumpBufferTimer = MoveStats.JumpBufferTime; // player can only jump when jump buffer is >0
            jumpReleasedDuringBuffer = false;
        }
        //when we release jump
        if (InputManager.JumpWasReleased)
        {
            if (jumpBufferTimer > 0f)
            {
                jumpReleasedDuringBuffer = true;
            }
            if (isJumping && VerticalVelocity > 0f) //enters the player into fastfall when they release the jump button before the end of the jump but after the apex
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = MoveStats.TimeForUpwardsCancel;
                    VerticalVelocity = 0f;
                }
                else // enters the player into fastfall when they release jump before the apex of the jump
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        //initiate jump with jump buffering and coyote time
        if (jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f)) //checks if the player is able to jump
        {
            InitiateJump(1);
            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        // double jump

        else if (jumpBufferTimer > 0f && isJumping && numOfJumpsUsed < MoveStats.NumberOfJumpsAllowed) 
        {
            isFastFalling = false;
            InitiateJump(1);
        }

        //air jump after coyote time
        else if (jumpBufferTimer > 0f && isFalling && numOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }

        //landing
        if ((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            numOfJumpsUsed = 0;

            VerticalVelocity = Physics2D.gravity.y;

            numOfJumpsUsed = 0;
        }
    }

    private void InitiateJump(int numberOfJumpsToUse)
    {
        if (!isJumping)
        {
            isJumping = true;
        }

        jumpBufferTimer = 0f;
        numOfJumpsUsed += numberOfJumpsToUse;
        VerticalVelocity = MoveStats.InitialJumpVelocity;
    }

    private void Jump()
    {
        //apply gravity
        if (isJumping)
        {
            //check if head bumbped
            if (bumpedHead)
            {
                isFastFalling = true;
            }
            //gravity on ascending
            if (VerticalVelocity >= 0f)
            {
                // apex controls
                apexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (apexPoint > MoveStats.ApexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }


                //gravity on ascending but not past apex threshold
                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }

            //gravity on descending
            else if (!isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultipler * Time.fixedDeltaTime; //can be tested without the gravityOnRelease multiplier
            }

            else if (VerticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                }
            }
        }

        // jump cut
        if (isFastFalling)
        {
            if (fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultipler * Time.fixedDeltaTime;
            }
            else if (fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / MoveStats.TimeForUpwardsCancel));
            }
            fastFallTime += Time.fixedDeltaTime;
        }
        // normal gravity after falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += MoveStats.Gravity * Time.fixedDeltaTime;
        }

        //clamp fall speed
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);

        rb.velocity = new Vector2(rb.velocity.x, VerticalVelocity);

    }

    #endregion

    #region Collision Checks

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x, MoveStats.GroundDetectionRayLength);

        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);
        if (groundHit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(feetCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(feetCollider.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);
        if (headHit.collider != null)
        {
            bumpedHead = true;
        }
        else { bumpedHead = false; }
    }


    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }


    #endregion

    #region Timers

    private void CountTimers()
    {
        //test if below methods work with time slow
        jumpBufferTimer -= Time.fixedDeltaTime;

        if (!isGrounded)
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }
        else { coyoteTimer = MoveStats.JumpCoyoteTime; }

    }

    #endregion

}

