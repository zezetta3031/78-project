using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    private int health = 3;
    private GameObject player;

    public void damage()
    {
        health -= 1;
        if (health == 0)
        {
            Destroy(player);
        }
    }
}
