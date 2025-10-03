using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject projectile;

    private void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(projectile);
    }

    private void OnBecameInvisible()
    {
        Destroy(projectile);
    }
}
