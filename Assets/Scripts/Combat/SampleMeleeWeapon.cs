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

        if (Input.mousePosition.x < Screen.width / 2f)
        {
            // perform left melee
            
        }
        else
        {
            // perform right melee
        }
        

        yield return new WaitForSeconds(meleeCooldown);
        _isMeleeInProgress = false;
    }
}

