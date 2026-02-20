using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
                        Vector2 direction = CalculateShotDirection(player.transform.position, transform.position);
                        // Instantiate projectile
                
                        Vector3 spawnPos = firePoint.position + (Vector3)(direction) + new Vector3(0f, 0.75f, 0f);
                        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                        // Set projectile rotation to face the direction
                        projectile.transform.rotation = CalculateShotRotation(direction.x, direction.y);
        
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
                        Vector2 direction = CalculateShotDirection(player.transform.position, transform.position);
                        // Instantiate projectile
                
                        Vector3 spawnPos = firePoint.position + (Vector3)(direction) + new Vector3(0f, 0.75f, 0f);
                        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                        // Set projectile rotation to face the direction
                        projectile.transform.rotation = CalculateShotRotation(direction.x, direction.y);
        
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

        switch (enemyType)
        {
            case EnemyType.Standard:
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
                break;
            case EnemyType.FirstBoss:
                // TODO: Make this slide on rail at random intervals and distances
            default:
                break;
        }
        
        
    }
    

    // TODO: Implement stuff based on the set value.
    public enum EnemyType
    {
        Standard,
        FirstBoss
    }

    public static Quaternion CalculateShotRotation(float x, float y)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(y, x) * Mathf.Rad2Deg);
    }

    public static Vector2 CalculateShotDirection(Vector3 playerTransform, Vector3 enemyTransform)
    {
        return (playerTransform - enemyTransform).normalized;
    }
}
