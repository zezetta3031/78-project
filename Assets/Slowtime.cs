using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Slowtime : MonoBehaviour
{
    public float SlowCooldown; //time left before slowtime can be used again
    public float slowTimer; // how long the time slow will last
    public bool slowingTime; // whether time is currently slowed

    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left shift") && SlowCooldown <= 0 && !slowingTime)
        {
            StartCoroutine(slowTime());
        }
        // Debug.LogFormat("slow timer: {0:F2}", slowTimer);
        // Debug.LogFormat("Cooldown: {0:F2}", SlowCooldown);
        // Debug.LogFormat("Slowing Time: {0:F2}", slowingTime);
    }


    IEnumerator slowTime()
    {
        if (SlowCooldown <= 0)
        {

            slowTimer = 3;
            slowingTime = true;
            while (slowTimer > 0)
            {
                Time.timeScale = 0.5f;
                slowTimer -= Time.deltaTime * (1 / Time.timeScale);
                yield return null;
            }
            SlowCooldown = 15;
            slowingTime = false;
            while (SlowCooldown > 0)
            {
                Time.timeScale = 1f;
                SlowCooldown -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
