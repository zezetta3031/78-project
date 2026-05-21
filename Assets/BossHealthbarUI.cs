using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbarUI : MonoBehaviour
{
   public EnemyScript enemyScript;
   public Image healthBar;
   public Gradient gradient;
   public float drainSpeed;


    void Awake()
    {
        SetMaxHealth(enemyScript.health);   
    }

    void Update()
    {
        SetHealth(enemyScript.health);
    }

    public void SetHealth(float Health)
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, Health, Time.deltaTime * drainSpeed);
        healthBar.color = gradient.Evaluate(healthBar.fillAmount);
    }

    public void SetMaxHealth(float Health)
    {

    }


}
