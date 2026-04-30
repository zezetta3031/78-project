using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameObject player;
    private HealthScript _healthScript;

    public void Awake()
    {
        player = GameObject.Find("PLAYER");
        _healthScript = player.GetComponent<HealthScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dialogue Trigger") || other.gameObject.name.Contains("Bullet") || other.CompareTag("Enemy Boundary"))
            return;
        if (other.CompareTag("Enemy"))
        {
            // Try to get the EnemyScript component on the object
            EnemyScript enemy = other.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.Inflict(0.25); // Apply damage
                _healthScript.EnemyKilled();
            }
            else
            {
                Debug.Log("No enemy script found on object tagged as enemy");
            }
        }

        if (other.CompareTag("Player"))
        {
            _healthScript.Damage();
        }
        
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
