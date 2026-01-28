using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMeleeWeapon : MonoBehaviour
{
    public float swipeDuration = 0.2f;    // How long the swipe lasts
    public float hitboxHeight = 0.0005f;
    public float meleeCooldown = 0.4f;    // Delay before you can swing again
    public float meleeRange = 2f;
    public GameObject meleeHitboxPrefab;


    private bool _isMeleeInProgress;
    public Transform firePoint;

    void Start()
    {
        firePoint = transform;
    }

    void Update()
    {
        // Middle mouse click (2). You can change to 0 or 1 for left/right clicks
        if (Input.GetMouseButtonDown(2) && !_isMeleeInProgress)
        {
            StartCoroutine(Melee());
        }
    }

    IEnumerator Melee()
    {
        _isMeleeInProgress = true;

        GameObject hitbox = null;

        try
        {
            float direction = (Input.mousePosition.x < Screen.width / 2f) ? -1f : 1f;

            Vector2 spawnPos =
                (Vector2)transform.position +                // player position (feet)
                Vector2.right * direction * (meleeRange * 0.5f) +  // forward based on direction
                Vector2.up * hitboxHeight;                  // static vertical offset
            
            hitbox = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

            var hb = hitbox.GetComponent<MeleeHitbox2D>();
            if (hb != null)
                hb.SetDirection(direction);
            else
                Debug.LogError("MeleeHitbox2D missing on prefab!");
        
            yield return new WaitForSecondsRealtime(swipeDuration);
        }
        finally
        {
            if (hitbox != null)
                Destroy(hitbox);

            _isMeleeInProgress = false;
        }

        yield return new WaitForSecondsRealtime(meleeCooldown);
    }
}

