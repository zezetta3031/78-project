using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleGunWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 500f;
    public Transform firePoint; // where the projectile spawns (can be the character's position)
    
    // Start is called before the first frame update
    void Start()
    {
        firePoint = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Get mouse position in world space
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        // Calculate direction from firePoint to mouse
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized * 1.1f;
        Vector3 spawnPos = firePoint.position + (Vector3)(direction) + new Vector3(0f, 0.75f, 0f);

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // Set projectile rotation to face the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Apply velocity
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.velocity = direction * projectileSpeed;
    }
}
