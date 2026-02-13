using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyScript : MonoBehaviour
{
    public double health = 1.0;
    public GameObject enemy;
    private new Renderer renderer;
    private GameObject player;
    public GameObject projectilePrefab;
    public float projectileSpeed = 2f;
    public Transform firePoint;
    public EnemyType enemyType;
    public DateTime timeOfLastShot = DateTime.Now;
    public GameObject enemyBoundaryLeft;
    public GameObject enemyBoundaryRight;
    private bool _isWalkingLeft; 

    public void Inflict(double dmg)
    {
        health -= dmg;
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (renderer.isVisible)
        {
            if (player.activeInHierarchy && timeOfLastShot.AddSeconds(0.5) < DateTime.Now)
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


            switch (enemyType)
            {
                case EnemyType.Standard:
                    // cool standard enemy stuff
                    break;
                case EnemyType.Boss:
                    // cool boss stuff
                    break;
                default:
                    Debug.Log("Unknown enemy type encountered. Why are you breaking stuff?");
                    break;
            }
        }
        
        if (health <= 0)
        {
            Destroy(enemy);
        }

        if (!enemyBoundaryLeft || !enemyBoundaryRight)
            return;
        
        if (_isWalkingLeft)
        {
            enemy.transform.position += Vector3.left * 0.001f;
        }
        else
        {
            enemy.transform.position += Vector3.right * 0.001f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (!other.CompareTag("Enemy Boundary"))
            return;
        
        _isWalkingLeft = !_isWalkingLeft;
    }

    // TODO: Implement stuff based on the set value.
    public enum EnemyType
    {
        Standard,
        Boss
    }
}
