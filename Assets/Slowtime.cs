using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slowtime : MonoBehaviour
{
    float SlowCooldown = 0;
    float slowTimer = 5;
    bool slowingTime = false;

    void Start()
    {
        SlowCooldown = 0;
        slowTimer = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left shift"))
        {
                slowTime();
        }
        if (slowTimer >= 5)
        {
                slowingTime = false;
        }
        Debug.Log(slowTimer);
        Debug.Log(SlowCooldown);
        Debug.Log(slowingTime);
        if (!slowingTime)
        {
            while (SlowCooldown > 0)
            {
                SlowCooldown -= Time.deltaTime;
            }
        }
    }


    public void slowTime()
    {
        if (SlowCooldown <= 0)
        {
            SlowCooldown = 15;
            slowTimer = 0;
            slowingTime = true;
            while (slowTimer < 5)
            {
                Time.timeScale = 0.5f;
                slowTimer += Time.deltaTime;
            }
            Debug.Log("got past");
        }
    }
}
