using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] PlayerMovement MovementScript;
    [SerializeField] PlayerAnimator AnimatorScript;
    
    [SerializeField] ParticleSystem WalkingParticles;
    [SerializeField] ParticleSystem WallSlideParticles;
    public bool isWallSliding;
    public bool isMoving;
    void Start()
    {
        
    }



    void Update()
    {
        PlayParticles();
        isWallSliding = MovementScript.isWallSliding;
        isMoving = AnimatorScript.isMoving;

        

    }

    public void PlayParticles()
    {
        if(isMoving && MovementScript.isGrounded && !WalkingParticles.isPlaying)
        {
            WalkingParticles.Play();
        }
        else if (WalkingParticles.isPlaying && (!MovementScript.isGrounded || !isMoving))
        {
            WalkingParticles.Stop();
        }

        if(isWallSliding && !WallSlideParticles.isPlaying)
        {
            WallSlideParticles.Play();
        }
        else if(!isWallSliding && WallSlideParticles.isPlaying)
        {
            WallSlideParticles.Stop();
        }

    }
}
