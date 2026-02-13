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
    public bool shouldMove;
    private DateTime timeOfLastShot = DateTime.Now;
    private DateTime timeOfLastBossBurst = DateTime.Now;
    private int bossBurstCycleCount = 0;
    public float enemyBoundaryLeft;
    public float enemyBoundaryRight;
    private bool _isWalkingLeft = true; 

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
            switch (enemyType)
            {
                case EnemyType.Standard:
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
                    break;
                case EnemyType.FirstBoss:
                    if (player.activeInHierarchy && (timeOfLastBossBurst.AddSeconds(3.0) < DateTime.Now || (timeOfLastShot.AddSeconds(0.1) < DateTime.Now && bossBurstCycleCount <= 3)))
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
                        
                        timeOfLastBossBurst = DateTime.Now;
                        bossBurstCycleCount++;
                    }
                    
                    if (bossBurstCycleCount == 3)
                        bossBurstCycleCount = 0;
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


        if (!shouldMove)
            return;
        
        if (_isWalkingLeft)
        {
            enemy.transform.position += Vector3.left * 0.001f;
        }
        else
        {
            enemy.transform.position += Vector3.right * 0.001f;
        }
        
        if (enemyBoundaryLeft >= enemy.transform.position.x && _isWalkingLeft) 
            _isWalkingLeft = false;
        else if (enemyBoundaryRight <= enemy.transform.position.x && !_isWalkingLeft)
            _isWalkingLeft = true;
    }
    

    // TODO: Implement stuff based on the set value.
    public enum EnemyType
    {
        Standard,
        FirstBoss
    }
}
