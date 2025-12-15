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
}
