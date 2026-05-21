using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Serialization;

public class BulletScript : MonoBehaviour
{
    public GameObject player;
    [FormerlySerializedAs("bossBullet")] public bool enemyBullet = false;
    private HealthScript _healthScript;
    private bool _touched;
    Vector2 bulletDir;
    Rigidbody2D rb;
    [SerializeField] ParticleSystem WallHitParticle;
    public float damage = 0.25f;
    public void Awake()
    {
        
        player = GameObject.Find("PLAYER");
        _healthScript = player.GetComponent<HealthScript>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dialogue Trigger") || other.gameObject.name.Contains("Bullet") || other.CompareTag("Enemy Boundary"))
            return;
        if (other.CompareTag("Enemy") && !enemyBullet)
        {
            // Try to get the EnemyScript component on the object
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.Inflict(damage); // Apply damage
                _healthScript.EnemyKilled();
            }
            else
            {
                Debug.Log("No enemy script found on object tagged as enemy");
            }
        }

        if (other.CompareTag("Player") && enemyBullet)
        {
            Debug.Log("Player hit bullet");
            _healthScript.Damage();
        }
        
        Destroy(gameObject);
    }

    private void OnCollisionExit2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Player"))
            return;

        if (_touched)
            Destroy(gameObject);

        if (!other.gameObject.CompareTag("Enemy"))
        {
            // Instantiate(WallHitParticle, gameObject.transform.position, gameObject.transform.rotation);
            _touched = true;
        }
        
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
