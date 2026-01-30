using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SampleMeleeWeapon : MonoBehaviour
{
    public float swipeDuration = 0.2f;    // How long the swipe lasts
    public float hitboxHeight = 0.0005f;
    public float meleeCooldown = 0.1f;    // Delay before you can swing again
    public float meleeRange = 2f;
    public CapsuleCollider2D col;
    public GameObject meleeHitboxPrefab;


    private bool _isMeleeInProgress;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !_isMeleeInProgress)
        {
            StartCoroutine(Melee());
        }
    }

    IEnumerator Melee()
    {
        _isMeleeInProgress = true;

        GameObject hitbox = null;
        
        Vector2 feetPos = new Vector2(
            col.bounds.center.x,
            col.bounds.min.y
        );

        try
        {
            float direction = (Input.mousePosition.x < Screen.width / 2f) ? -1f : 1f;

            var spawnPos =
                feetPos +                // player position (feet)
                Vector2.right * direction * (meleeRange * 0.5f) +  // forward based on direction
                Vector2.up * hitboxHeight;                  // static vertical offset
            
            hitbox = Instantiate(meleeHitboxPrefab, spawnPos, Quaternion.identity);

            var hb = hitbox.GetComponent<MeleeHitbox2D>();
            if (!hb.IsUnityNull())
                hb.SetDirection(direction);
            else
                Debug.LogError("MeleeHitbox2D missing on prefab!");
            
            Debug.Log(
                $"SpawnY: {spawnPos.y} | " +
                $"TransformY: {hitbox.transform.position.y} | " +
                $"FeetY: {feetPos.y}"
            );
        
            yield return new WaitForSecondsRealtime(swipeDuration);
        }
        finally
        {
            if (!hitbox.IsUnityNull())
                Destroy(hitbox);

            _isMeleeInProgress = false;
        }

        yield return new WaitForSecondsRealtime(meleeCooldown);
    }
}

