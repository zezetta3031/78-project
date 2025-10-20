using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // private int _framesInsideObject = 0;
    
    // private void OnCollisionStay2D(Collision2D other)
    // {
    //     _framesInsideObject++;
    //     if (_framesInsideObject >= 10)
    //         Destroy(gameObject);
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dialogue Trigger") || other.CompareTag("Player") || other.gameObject.name.Contains("Bullet"))
            return;
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
