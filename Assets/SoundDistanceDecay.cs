using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDistanceDecay : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform player;
    public float maxDistance = 15f;



    void Update()
    {
        
        float distance = Vector3.Distance(player.position, transform.position);
        float volume = Mathf.Clamp01(1 - (distance / maxDistance));

        audioSource.volume = volume;
        
    }
}
