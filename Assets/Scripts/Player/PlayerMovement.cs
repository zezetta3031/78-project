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
    public Animator animator;

    private Rigidbody2D rb;

    public bool playerFreeze;

    //Movement variables
    public float HorizontalVelocity { get; private set; }
    public bool isFacingRight{ get; private set; }

    //Collision check variables
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private RaycastHit2D wallHit;
    private RaycastHit2D lastWallHit;
    public bool isGrounded { get; private set; }
    private bool bumpedHead;
    private bool isTouchingWall;

    //jump variables
    public float VerticalVelocity { get; private set; }
    public bool isJumping { get; private set; }
    public bool isFastFalling { get; private set; } //is true when the player releases jump before they reach the apex of their jump
    public bool isFalling { get; private set; }
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

    //wall slide vars
    public bool isWallSliding;
    public bool isWallSlideFalling;

    //wall jump vars
    private bool useWallJumpMoveStats;
    public bool isWallJumping;
    private float wallJumpTime;
    public bool isWallJumpFastFalling;
    public bool isWallJumpFalling;
    private float wallJumpFastFallTime;
    private float wallJumpFastFallReleaseSpeed;

    private float wallJumpPostBufferTimer;

    private float wallJumpApexHeight;
    private float timePastWallJumpApexThreshold;
    private bool isPastWallJumpApexThreshold;

    //dash vars
    private bool isDashing;
    private bool isAirDashing;
    private float dashTimer;
    private float dashOnGroundTimer;
    private int numberOfDashesUsed;
    private Vector2 dashDirection;
    private bool isDashFastFalling;
    private float dashFastFallTime;
    private float dashFastFallReleaseSpeed;

    [Header("SFX")]
    [SerializeField] public AudioClip JumpSound;
    [SerializeField] public AudioClip DoubleJumpSound;
    [SerializeField] public AudioClip LandingSFX;
    private AudioSource audioSource; 


    private void Awake()
    {
        isFacingRight = true;

        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        CountTimers();
        JumpChecks();
        LandCheck();

        WallSlideCheck();
        WallJumpCheck();
    }

    private void FixedUpdate()
    {
        CollisionChecks();
        Jump();
        Fall();
        WallSlide();
        WallJump();

        if (isGrounded)
        {
            Move(MoveStats.GroundAcceleration, MoveStats.GroundDeceleration, InputManager.Movement);
        }
        else
        {
            if (useWallJumpMoveStats)
            {
                Move(MoveStats.WallJumpMovementAcceleration, MoveStats.WallJumpMovementDeceleration, InputManager.Movement);
            }
            Move(MoveStats.AirAcceleration, MoveStats.AirDeceleration, InputManager.Movement);
        }

        ApplyVelocity();
    }

    private void ApplyVelocity()
    {
        //Clamp fall speed
        if (!isDashing)
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f);
        }
        else
        {
            VerticalVelocity = Mathf.Clamp(VerticalVelocity, -50f, 50f);
        }


       

        rb.velocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        if (playerFreeze)
        {
            HorizontalVelocity = 0f;

        }
        else if (!isDashing)
        {
            if (moveInput != Vector2.zero)
            {
                TurnCheck(moveInput);

                float targetVelocity = 0f;
                if (InputManager.RunIsHeld)
                {
                    targetVelocity = (moveInput.x) * MoveStats.MaxRunSpeed;
                }
                else { targetVelocity = (moveInput.x) * MoveStats.MaxWalkSpeed; }

                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.deltaTime);
            }
            else if (moveInput == Vector2.zero)
            {
                HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.deltaTime);
            }
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

    #region Land/Fall

    private void LandCheck()
    {
         //landing
        if ((isJumping || isFalling || isWallJumpFalling || isWallJumping || isWallSlideFalling || isWallSliding || isDashFastFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            // Debug.Log("test");
            ResetJumpValues();
            StopWallSlide();
            ResetWallJumpValues();
            ResetDashes();

            VerticalVelocity = Physics2D.gravity.y;

            numOfJumpsUsed = 0;

            if(isDashFastFalling && isGrounded)
            {
                ResetDashValues();
                return;
            }

            ResetDashValues();
        }
    }

    private void Fall()
    {
         // normal gravity after falling
        if (!isGrounded && !isJumping && !isWallSliding && !isDashing && !isWallJumping && !isDashFastFalling)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += MoveStats.Gravity * Time.deltaTime;
        }
    }

    #endregion

    #region Jump


    private void ResetJumpValues()
    {
        audioSource.clip = LandingSFX;
        audioSource.Play();
        isJumping = false;
        isFalling = false;
        isFastFalling = false;
        fastFallTime = 0f;
        isPastApexThreshold = false;
    }
    private void JumpChecks()
    {
        //When we press jump
        if (InputManager.JumpWasPressed)
        {
            if( isWallSlideFalling && wallJumpPostBufferTimer >= 0f)
            {
                return;
            }
            else if(isWallSliding || (isTouchingWall && !isGrounded))
            {
                return;
            }

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
        if (jumpBufferTimer > 0f && !isFalling && (isGrounded || coyoteTimer > 0f)) //checks if the player is able to jump
        {
            InitiateJump(1);
            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        // double jump

        else if (jumpBufferTimer > 0f && (isJumping || isWallSliding || isWallSlideFalling || isAirDashing || isDashFastFalling) && !isTouchingWall && isFalling && numOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            isFalling = false;
            InitiateJump(1);

            if (isDashFastFalling)
            {
                isDashFastFalling = false;
            }
        }

        //air jump after coyote time
        else if (jumpBufferTimer > 0f && !isWallSlideFalling && isFalling && numOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1)
        { 
            InitiateJump(2);
            isFastFalling = false;
        }

       
    }

    private void InitiateJump(int numberOfJumpsToUse)
    {
        if (!playerFreeze)
        {
            if (!isJumping)
            {
                isJumping = true;
                audioSource.clip = JumpSound;
                audioSource.Play();
                Debug.Log("jump test");
            }

            ResetWallJumpValues();

            jumpBufferTimer = 0f;
            numOfJumpsUsed += numberOfJumpsToUse;
            VerticalVelocity = MoveStats.InitialJumpVelocity;
            animator.SetTrigger("JumpTrigger");
        }
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
                        timePastApexThreshold += Time.deltaTime;
                        if (timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                            isJumping = false;
                            isFalling = true;
                        }
                    }
                }


                //gravity on ascending but not past apex threshold
                else
                {
                    VerticalVelocity += MoveStats.Gravity * Time.deltaTime;
                    if (isPastApexThreshold)
                    {
                        isPastApexThreshold = false;
                    }
                }
            }

            //gravity on descending
            else if (!isFastFalling)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultipler * Time.deltaTime; //can be tested without the gravityOnRelease multiplier
            }

            else if (VerticalVelocity < 0f)
            {
                if (!isFalling)
                {
                    isFalling = true;
                    isJumping = false;
                }
            }
        }

        // jump cut
        if (isFastFalling)
        {
            if (fastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMultipler * Time.deltaTime;
            }
            else if (fastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / MoveStats.TimeForUpwardsCancel));
            }
            fastFallTime += Time.deltaTime;
        }
       


    }

    #endregion

    #region Wall Slide

    private void WallSlideCheck()
    {
        if (isTouchingWall && !isGrounded && !isDashing)
        {
            if (VerticalVelocity < 0f && !isWallSliding)
            {
                ResetJumpValues();
                ResetWallJumpValues();
                ResetDashValues();

                isWallSlideFalling = false;  
                isWallSliding = true;

                if (MoveStats.ResetJumpOnWallSlide)
                {
                    numOfJumpsUsed = 0;
                }
                if (MoveStats.ResetDashOnWallSlide)
                {
                    ResetDashes();   
                }
            }
        }

        else if (isWallSliding && !isTouchingWall && !isGrounded && !isWallSlideFalling)
        {
            isWallSlideFalling = true;
            StopWallSlide();
        }

        else
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if (isWallSliding)
        {
            numOfJumpsUsed++;
            isWallSliding = false;
        }
        
    }

    private void WallSlide()
    {
        if (isWallSliding)
        {
            VerticalVelocity = Mathf.Lerp(VerticalVelocity, -MoveStats.WallSlideSpeed, MoveStats.WallSlideDecelerationSpeed * Time.deltaTime);

        }
    }

    #endregion

    #region Wall Jump

    private void ResetWallJumpValues()
    {
        isWallSliding = false;
        isWallSlideFalling = false;
        useWallJumpMoveStats = false;
        isWallJumping = false;
        isWallJumpFastFalling = false;
        isWallJumpFalling = false;
        isPastWallJumpApexThreshold = false;

        wallJumpFastFallTime = 0f;
        wallJumpTime = 0f;
    }

    private void WallJumpCheck()
    {
        if (ShouldApplyPostWallJumpBuffer())
        {
            wallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }

        //wall jump fast falling
        if (InputManager.JumpWasReleased && !isWallSliding && !isTouchingWall && isWallJumping)
        {
            if(VerticalVelocity > 0f)
            {
                if (isPastWallJumpApexThreshold)
                {
                    
                    isPastWallJumpApexThreshold = false;
                    isWallJumpFastFalling = true;
                    wallJumpFastFallTime = MoveStats.TimeForUpwardsCancel;

                    VerticalVelocity = 0f;
                }
                else
                {
                    isWallJumpFastFalling = true;
                    wallJumpFastFallReleaseSpeed = VerticalVelocity; 
                }
            }
        }
        //actual jump with post wall jump buffer time
        if(InputManager.JumpWasPressed && wallJumpPostBufferTimer > 0f)
        {
            InitiateWallJump();
        }
    }

    private void InitiateWallJump()
    {
        if (!isWallJumping)
        {
            isWallJumping = true;
            useWallJumpMoveStats = true;
        }

        StopWallSlide();
        ResetJumpValues();
        wallJumpTime = 0f;

        VerticalVelocity = MoveStats.InitialWallJumpVelocity;

        int dirMultiplier = 0; //Direction multiplier (whether to move in positive or negative X direction (left and right))
        Vector2 hitPoint = lastWallHit.collider.ClosestPoint(bodyCollider.bounds.center);

        if(hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        else{ dirMultiplier = 1; }

        HorizontalVelocity = Mathf.Abs(MoveStats.WallJumpDirection.x) * dirMultiplier;

    }

    public void WallJump()
    {
        //APPLY WALL JUMP GRAVITY
        if (isWallJumping)
        {
             //time to take over movement controls while wall jumping
             wallJumpTime += Time.fixedDeltaTime;
             if(wallJumpTime >= MoveStats.TimeTillJumpApex)
            {
                useWallJumpMoveStats = false;
            }

            //hit head
            if (bumpedHead)
            {
                isWallJumpFastFalling = true;
                useWallJumpMoveStats = false;
            }

            //gravity on ascent
            if(VerticalVelocity >= 0f)
            {
                //apex controls
                wallJumpApexHeight = Mathf.InverseLerp(MoveStats.WallJumpDirection.y, 0f, VerticalVelocity);

                if(wallJumpApexHeight > MoveStats.ApexThreshold)
                {
                    if (!isPastWallJumpApexThreshold)
                    {
                        isPastWallJumpApexThreshold = true;
                        timePastWallJumpApexThreshold = 0f;
                    }

                    if (isPastWallJumpApexThreshold)
                    {
                        timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if(timePastWallJumpApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }

                //gravity on ascent but not past apex
                else if (!isWallJumpFastFalling)
                {
                    VerticalVelocity += MoveStats.WallJumpGravity * Time.fixedDeltaTime;

                    if (isPastWallJumpApexThreshold)
                    {
                        isPastWallJumpApexThreshold = false;
                    }
                }
                
            }
            //Gravity on descending
            else if (!isWallJumpFastFalling)
            {
                VerticalVelocity += MoveStats.WallJumpGravity * Time.deltaTime;
            }

            else if( VerticalVelocity < 0f)
            {
                if (!isWallJumpFalling)
                {
                    isWallJumpFalling = true;
                }
            }
        }

        //Handle wall jump cut time
        if (isWallJumpFastFalling)
        {
            if(wallJumpFastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity += MoveStats.WallJumpGravity * MoveStats.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (wallJumpFastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                VerticalVelocity = Mathf.Lerp(wallJumpFastFallReleaseSpeed, 0f, (wallJumpFastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            wallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if(!isGrounded && (isTouchingWall || isWallSliding))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #endregion

    #region Dash

    private void ResetDashValues()
    {
        isDashFastFalling = false;
        dashOnGroundTimer = -0.0f;
    }

    private void ResetDashes()
    {
        numberOfDashesUsed = 0;
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

    private void IsTouchingWall()
    {
        float originEndPoint = 0f;
        if (isFacingRight)
        {
            originEndPoint = bodyCollider.bounds.max.x;
        }
        else{ originEndPoint = bodyCollider.bounds.min.x; }

        float adjustedHeight = bodyCollider.bounds.size.y * MoveStats.WallDetectionRayHeightMultiplier;

        Vector2 boxCastOrigin = new Vector2(originEndPoint, bodyCollider.bounds.center.y);
        Vector2 boxCastSize = new Vector2(MoveStats.WallDetectionRayLength, adjustedHeight);

        wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, MoveStats.WallDetectionRayLength, MoveStats.GroundLayer);
        if(wallHit.collider != null)
        {
            lastWallHit = wallHit;
            isTouchingWall = true;
        }
        else { isTouchingWall = false; }
        #region Debug Visualization

        if (MoveStats.DebugShowWallHitbox)
        {
            Color rayColor;
            if (isTouchingWall)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y /2);
            Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y /2);
            Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y /2);
            Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y /2);

            Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
            Debug.DrawLine(boxBottomRight, boxTopRight, rayColor);
            Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
            Debug.DrawLine(boxTopLeft, boxBottomLeft, rayColor);

        }

         #endregion
    }

    


    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
        IsTouchingWall();
    }


    #endregion

    #region Timers

    private void CountTimers()
    {

        //Jump buffer
        jumpBufferTimer -= Time.deltaTime;

        //coyote time
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else { coyoteTimer = MoveStats.JumpCoyoteTime; }

        // wall jump buffer time
        if (!ShouldApplyPostWallJumpBuffer())
        {
            wallJumpPostBufferTimer -= Time.deltaTime;
        }

    }

    #endregion
}