using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] PlayerMovement MovementScript;
    [SerializeField] ParticleSystem WalkingParticles;
    [SerializeField] ParticleSystem WallSlideParticles;
    public bool isWallSliding;
    void Start()
    {
        
    }



    void Update()
    {
        isWallSliding = MovementScript.isWallSliding;

        PlayParticles();

    }

    public void PlayParticles()
    {
        if (isWallSliding && !WallSlideParticles.isPlaying)
        {
            WallSlideParticles.Play();
            Debug.Log(WallSlideParticles.isPlaying);
        }
        else
        {
            WallSlideParticles.Stop();
        }
    }
}
