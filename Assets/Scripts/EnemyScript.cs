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
    private float timeOfLastBossBurst = 0f;
    private float timeOfLastShot = 0f;
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
                    if (player.activeInHierarchy && Time.time > timeOfLastShot + 0.5f)
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
                        timeOfLastShot = Time.time;
                    }
                    break;
                case EnemyType.FirstBoss:
                    if (!player.activeInHierarchy) return;

                    // Start a new burst every 3 seconds
                    if (Time.time > timeOfLastBossBurst + 3f)
                    {
                        bossBurstCycleCount = 0;
                        timeOfLastBossBurst = Time.time;
                    }

                    // Fire shots within the burst (max 3 shots, 0.1s apart)
                    if (bossBurstCycleCount < 3 && Time.time > timeOfLastShot + 0.1f)
                    {
                        Vector2 direction = CalculateShotDirection(player.transform.position, transform.position);

                        Vector3 spawnPos = firePoint.position + (Vector3)(direction) + new Vector3(0f, 0.75f, 0f);
                        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                        // Rotate projectile
                        projectile.transform.rotation = CalculateShotRotation(direction.x, direction.y);

                        // Apply velocity
                        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                        rb.velocity = direction * projectileSpeed;

                        timeOfLastShot = Time.time;
                        bossBurstCycleCount++;
                    }
                    
                    if (bossBurstCycleCount == 3)
                        bossBurstCycleCount = 0;
                    break;
                case EnemyType.Shotgun:
                    if (player.activeInHierarchy && Time.time > timeOfLastShot + 1.2f)
                    {
                        Vector2 baseDirection = CalculateShotDirection(player.transform.position, transform.position);
                        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

                        int pelletCount = 5;
                        float spreadAngle = 30f; // total spread in degrees

                        for (int i = 0; i < pelletCount; i++)
                        {
                            float t = (float)i / (pelletCount - 1);
                            float angle = baseAngle + Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, t);
                            float rad = angle * Mathf.Deg2Rad;
                            Vector2 pelletDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                            Vector3 spawnPos = firePoint.position + (Vector3)pelletDirection + new Vector3(0f, 0.75f, 0f);
                            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                            projectile.transform.rotation = CalculateShotRotation(pelletDirection.x, pelletDirection.y);

                            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                            rb.velocity = pelletDirection * projectileSpeed;
                        }

                        timeOfLastShot = Time.time;
                    }
                    break;
                default:
                    Debug.Log("Unknown enemy type encountered. Why are you breaking stuff?");
                    break;
            }
        }
        
        if (health <= 0)
        {
            if (enemyType == EnemyType.FirstBoss)
            {
                HealthScript hs = GetComponent<HealthScript>();
                if (hs != null)
                {
                    hs.health = 3;
                }
            }
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
        Shotgun,
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
