using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public int health = 3;
    private int _lastFrameHeath = 3;
    public GameObject secondHeart;
    public GameObject thirdHeart;
    public Sprite emptyHeart;
    public Sprite fullHeart;

    private int enemiesKilledStreak = 0;

    public void Damage()
    {
        health -= 1;
        if (health == 2)
        {
            thirdHeart.GetComponent<Image>().sprite = emptyHeart;
        } else if (health == 1)
        {
            secondHeart.GetComponent<Image>().sprite = emptyHeart;
        }
        else if (health == 0)
        {
            Debug.Log("Player is dead");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Recharge()
    {
        if (health != 3)
            health += 1;
        if (health == 2)
        {
            secondHeart.GetComponent<Image>().sprite = fullHeart;
            thirdHeart.GetComponent<Image>().sprite = emptyHeart;
        } else if (health == 3)
        {
            secondHeart.GetComponent<Image>().sprite = fullHeart;
            thirdHeart.GetComponent<Image>().sprite = fullHeart;
        } 
    }

    public void Update()
    {
        if (health > _lastFrameHeath)
        {
            if (health == 2)
            {
                secondHeart.GetComponent<Image>().sprite = fullHeart;
                thirdHeart.GetComponent<Image>().sprite = emptyHeart;
            } else if (health == 3)
            {
                secondHeart.GetComponent<Image>().sprite = fullHeart;
                thirdHeart.GetComponent<Image>().sprite = fullHeart;
            } 
        }
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
