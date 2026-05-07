using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthPowerupSpawner : MonoBehaviour
{
    private bool _powerupActive;
    private float _lastPowerupSpawn = Time.time;
    private GameObject _powerup;
    public GameObject[] powerups;

    // Update is called once per frame
    void Update()
    {
        if (!_powerupActive && Time.time - _lastPowerupSpawn > 15)
        {
            var idx = Random.Range(0, powerups.Length);
            powerups[idx].SetActive(true);
            _powerupActive = true;
            _powerup = powerups[idx];
        }
    }

    public void DespawnPowerup()
    {
        _powerupActive = false;
        _powerup.SetActive(false);
    }
}
