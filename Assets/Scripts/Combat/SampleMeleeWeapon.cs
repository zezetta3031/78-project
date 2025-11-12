using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMeleeWeapon : MonoBehaviour
{
    public float swipeAngle = 90f;        // Total angle of the swipe
    public float swipeDuration = 0.2f;    // How long the swipe lasts
    public float meleeCooldown = 0.4f;    // Delay before you can swing again
    public float meleeRange = 1f;
    public GameObject meleeVisualPrefab;

    private bool isMeleeInProgress;
    public Transform firePoint;

    void Start()
    {
        firePoint = transform;
    }

    void Update()
    {
        // Middle mouse click (2). You can change to 0 or 1 for left/right clicks
        if (Input.GetMouseButtonDown(2) && !isMeleeInProgress)
        {
            StartCoroutine(Melee());
        }
    }

    IEnumerator Melee()
    {
        isMeleeInProgress = true;

        // --- Determine the direction to the mouse ---
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;
        Vector2 spawnPos = (Vector2)(firePoint.position) + direction + new Vector2(0f, 0f);

        // --- Determine base angle facing the mouse ---
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // --- Instantiate the melee visual (your bullet prefab) ---
        GameObject visual = Instantiate(meleeVisualPrefab, spawnPos, Quaternion.identity);
        visual.transform.localScale *= 2f; // optional, makes it more visible
        Rigidbody2D rb = visual.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = false; // disable physics

        Debug.Log("Melee started");

        // --- Swing in an arc ---
        float elapsed = 0f;
        while (elapsed < swipeDuration)
        {
            float t = elapsed / swipeDuration;
            float currentAngle = Mathf.Lerp(-swipeAngle / 2f, swipeAngle / 2f, t);

            // Compute the position of the visual offset from the player in an arc
            float totalAngle = baseAngle + currentAngle;
            Vector3 offset = new Vector3(Mathf.Cos(totalAngle * Mathf.Deg2Rad),
                Mathf.Sin(totalAngle * Mathf.Deg2Rad),
                0f) * meleeRange;
            
            if (offset.y < 0f)
                offset.y = 0f;

            visual.transform.position = spawnPos + (Vector2)(offset);
            visual.transform.rotation = Quaternion.Euler(0, 0, totalAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(visual);
        Debug.Log("Melee finished");

        yield return new WaitForSeconds(meleeCooldown);
        isMeleeInProgress = false;
    }
}

