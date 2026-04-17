using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleGunWeapon : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 2f;
    public Transform firePoint;

    [SerializeField] PlayerMovement PlayerMovementScript;

    private float _lastShotTime = 0f;
    private float _mouseHoldDuration = 0f;

    // Fire rate settings
    private const float BASE_FIRE_INTERVAL = 0.5f;   // seconds between shots at the start
    private const float MIN_FIRE_INTERVAL  = 0.08f;  // fastest possible fire rate
    private const float RAMP_DURATION      = 3f;   // seconds to reach max fire rate

    void Start()
    {
        firePoint = transform;
    }

    void Update()
    {
        if (PlayerMovementScript.playerFreeze) return;

        if (Input.GetMouseButton(0))
        {
            _mouseHoldDuration = Mathf.Min(_mouseHoldDuration + Time.deltaTime, RAMP_DURATION);

            float t = Mathf.Clamp01(_mouseHoldDuration / RAMP_DURATION);
            float fireInterval = Mathf.Lerp(BASE_FIRE_INTERVAL, MIN_FIRE_INTERVAL, t);

            if (_mouseHoldDuration < RAMP_DURATION)
            {
                if (Time.time - _lastShotTime >= fireInterval)
                {
                    Shoot();
                    _lastShotTime = Time.time;
                }
            }
        }
        else
        {
            // Reset ramp when mouse is released
            _mouseHoldDuration = 0f;
        }
    }

    void Shoot()
    {
        Vector3 mouseWorldPos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;
        Vector3 spawnPos = firePoint.position + (Vector3)direction;

        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.velocity = direction * projectileSpeed;
    }
}
