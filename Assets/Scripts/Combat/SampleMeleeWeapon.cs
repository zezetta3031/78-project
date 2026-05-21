using System.Collections;
using UnityEngine;

public class SampleMeleeWeapon : MonoBehaviour
{
    public float swipeDuration = 0.2f;
    public float meleeCooldown = 0.1f;
    public float meleeRange = 2f;
    public Vector2 hitboxSize = new Vector2(1f, 1.5f);
    public Vector2 hitboxOffset = new Vector2(0f, 1f);

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

        try
        {
            float direction = (Input.mousePosition.x < Screen.width / 2f) ? -1f : 1f;

            Vector2 spawnPos = (Vector2)transform.position +
                               direction * (meleeRange * 1.5f) * Vector2.right +
                               hitboxOffset;

            hitbox = CreateHitbox(spawnPos, direction);

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

    private GameObject CreateHitbox(Vector2 position, float direction)
    {
        GameObject hitbox = new GameObject("MeleeHitbox");
        hitbox.transform.position = position;
        hitbox.layer = gameObject.layer;

        BoxCollider2D boxCol = hitbox.AddComponent<BoxCollider2D>();
        boxCol.size = hitboxSize;
        boxCol.isTrigger = true;

        MeleeHitbox2D hb = hitbox.AddComponent<MeleeHitbox2D>();
        hb.SetDirection(direction);

        return hitbox;
    }
}