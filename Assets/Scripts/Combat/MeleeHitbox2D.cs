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
    
    public void SetDirection(float direction)
    {
        col.offset = new Vector2(Mathf.Abs(col.offset.x) * direction, col.offset.y);
    }
}
