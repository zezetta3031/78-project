using System;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject player;
    public bool bossBullet = false;
    private HealthScript _healthScript;
    private bool _touched;

    public void Awake()
    {
        player = GameObject.Find("PLAYER");
        _healthScript = player.GetComponent<HealthScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dialogue Trigger") || other.gameObject.name.Contains("Bullet") || other.CompareTag("Enemy Boundary"))
            return;
        if (other.CompareTag("Enemy") && !bossBullet)
        {
            // Try to get the EnemyScript component on the object
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.Inflict(0.25); // Apply damage
                _healthScript.EnemyKilled();
            }
            else
            {
                Debug.Log("No enemy script found on object tagged as enemy");
            }
        } else if (other.CompareTag("Enemy") && bossBullet)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit bullet");
            _healthScript.Damage();
        }
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_touched) 
            Destroy(gameObject);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        _touched = true;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
