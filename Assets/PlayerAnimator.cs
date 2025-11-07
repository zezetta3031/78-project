using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    private PlayerMovement MovementScript;
    private bool isMoving;
    void Start()
    {
        MovementScript = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        AnimationUpdate(animator);

    }
    
       public void AnimationUpdate(Animator animator){
        animator.SetFloat("AnimVelocityX", MovementScript.moveVelocity.x);
        animator.SetBool("AnimIsFalling", MovementScript.isFalling);
        animator.SetBool("AnimIsGrounded", MovementScript.isGrounded);
        animator.SetBool("AnimIsFastFalling", MovementScript.isFastFalling);
        animator.SetBool("AnimIsJumping", MovementScript.isJumping);
        if (MovementScript.moveVelocity.x > 1 || MovementScript.moveVelocity.x < -1)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
        animator.SetBool("AnimIsMoving", isMoving);
        if (MovementScript.isFacingRight)
        {
            animator.SetFloat("isFacingRight", 1);
        }
        else
        {
            animator.SetFloat("isFacingRight", 0);
        }

    }
}
