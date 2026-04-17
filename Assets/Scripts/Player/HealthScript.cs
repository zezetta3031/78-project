using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    private int _health = 3;
    public GameObject player;
    public GameObject firstHeart;
    public GameObject secondHeart;
    public GameObject thirdHeart;
    public Sprite emptyHeart;

    private int enemiesKilledStreak = 0;

    public void Damage()
    {
        _health -= 1;
        if (_health == 2)
        {
            thirdHeart.GetComponent<Image>().sprite = emptyHeart;
        } else if (_health == 1)
        {
            secondHeart.GetComponent<Image>().sprite = emptyHeart;
        }
        else if (_health == 0)
        {
            Debug.Log("Player is dead");
            Destroy(player);
        }
    }

    public void Recharge()
    {
        _health += 1;
        if (_health == 2)
        {
            thirdHeart.GetComponent<Image>().sprite = emptyHeart;
        } else if (_health == 1)
        {
            secondHeart.GetComponent<Image>().sprite = emptyHeart;
        }
    }

    public void Update()
    {
        if (enemiesKilledStreak == 10)
        {
            enemiesKilledStreak = 0;
            Recharge();
        }
    }

    public void EnemyKilled()
    {
        enemiesKilledStreak++;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Health Powerup"))
        {
            Recharge();
        }
    }
}
