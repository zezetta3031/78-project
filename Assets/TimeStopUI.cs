using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimeStopUI : MonoBehaviour
{
    public Slowtime slowTimeScript;
    [SerializeField] public Image fillCircle;
    private float maxCooldownValue;
    private float maxTimerValue;

    public void Start()
    {

    }

    public void Update()
    {
        if (slowTimeScript.slowingTime)
        {
            fillCircle.color = new Color32(112,255,188,255);
            fillCircle.fillAmount = slowTimeScript.slowTimer / 3;
        }
        else
        {
            fillCircle.color = new Color32(133, 222, 226, 255);
            fillCircle.fillAmount = slowTimeScript.SlowCooldown / 15;
        }

        Debug.Log(slowTimeScript.SlowCooldown);
        Debug.Log(fillCircle.fillAmount);
    }

}
