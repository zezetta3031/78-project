using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMeleeWeapon : MonoBehaviour
{
    private bool isMeleeInProgress;
    private Transform firePoint;
    
    // Start is called before the first frame update
    void Start()
    {
        isMeleeInProgress = false;
        firePoint = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2) && !isMeleeInProgress)
        {
            isMeleeInProgress = true;
            Melee();
        }
    }

    void Melee()
    {
        Debug.Log("Melee");
        isMeleeInProgress = false;
    }
}
