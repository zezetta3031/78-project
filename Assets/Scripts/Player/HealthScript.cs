using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    private int _health = 3;
    public GameObject player;

    public void Damage()
    {
        _health -= 1;
        if (_health == 0)
        {
            Debug.Log("Player is dead");
            Debug.Log(player);
            Destroy(player);
        }
    }
}
