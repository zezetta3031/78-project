using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public double health = 1.0;
    public GameObject enemy;

    public void Inflict(double dmg)
    {
        health -= dmg;
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(enemy);
        }
    }
}
