using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneTimeParticle : MonoBehaviour
{
    private ParticleSystem particleSystem;
    void Awake()
    {
        particleSystem = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (particleSystem)
        {
            if (!particleSystem.IsAlive())
            {
                Destroy(gameObject,0.6f);
            }
        }
        
    }
}
