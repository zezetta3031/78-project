using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox2D : MonoBehaviour
{
    private BoxCollider2D col;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    void OnDrawGizmos()
    {
        if (col == null) return;

        Gizmos.color = Color.red;

        // Convert collider center to world space
        Vector2 worldCenter = (Vector2)transform.position +
                              (Vector2)(transform.rotation * col.offset);

        // Convert collider size to world space
        Vector2 worldSize = Vector2.Scale(col.size, transform.lossyScale);

        Gizmos.DrawWireCube(worldCenter, worldSize);
    }
    
    public void SetDirection(float direction, float playerYPos)
    {
        col.offset = new Vector2(Mathf.Abs(col.offset.x) * direction, playerYPos);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dialogue Trigger") || other.gameObject.name.Contains("Bullet"))
            return;
        EnemyScript enemy = other.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            enemy.Inflict(0.25); // Apply damage
            Debug.Log("Detected enemy");
        }
        else
        {
            Debug.Log("No enemy script found on object tagged as enemy");
        }
    }
}
