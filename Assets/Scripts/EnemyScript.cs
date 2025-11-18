using System;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public double health = 1.0;
    public GameObject enemy;
    public Renderer renderer;
    public GameObject projectilePrefab;
    public float projectileSpeed = 2f;
    public Transform firePoint;
    public DateTime timeOfLastShot = DateTime.Now;

    public void Inflict(double dmg)
    {
        health -= dmg;
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (renderer.isVisible)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player.activeInHierarchy && timeOfLastShot.AddSeconds(1.0) < DateTime.Now)
            {
                Vector2 direction = player.transform.position - transform.position;
                direction.Normalize();
                // Instantiate projectile
                
                Vector3 spawnPos = firePoint.position + (Vector3)(direction) + new Vector3(0f, 0.75f, 0f);
                GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                // Set projectile rotation to face the direction
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        
                // Apply velocity
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.velocity = direction * projectileSpeed;
                timeOfLastShot = DateTime.Now;
            }
        }
        if (health <= 0)
        {
            Destroy(enemy);
        }
    }
}
