using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dialogue Trigger") || other.CompareTag("Player") || other.gameObject.name.Contains("Bullet"))
            return;
        if (other.CompareTag("Enemy"))
        {
            // Try to get the EnemyScript component on the object
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.Inflict(0.25); // Apply damage
            }
            else
            {
                Debug.Log("No enemy script found on object tagged as enemy");
            }
        }
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
