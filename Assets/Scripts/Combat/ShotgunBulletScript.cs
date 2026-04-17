using UnityEngine;

namespace Combat
{
    public class ShotgunBulletScript : MonoBehaviour
    {
        [Header("Shotgun Split Settings")]
        public GameObject bulletPrefab;     
        public int pelletCount = 5;
        public float spreadAngle = 30f;
        public float splitDistance = 4f;    // splits after travelling this far
        public float splitTime = 1.5f;      // or after this many seconds, whichever comes first

        private Vector3 _spawnPosition;
        private float _spawnTime;
        private bool _hasSplit;

        private void Start()
        {
            _spawnPosition = transform.position;
            _spawnTime = Time.time;
        }

        private void Update()
        {
            if (_hasSplit) return;

            bool distanceReached = Vector3.Distance(transform.position, _spawnPosition) >= splitDistance;
            bool timeReached = Time.time - _spawnTime >= splitTime;

            if (distanceReached || timeReached)
                Split();
        }

        private void Split()
        {
            _hasSplit = true;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            float baseAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            float speed = rb.velocity.magnitude;

            for (int i = 0; i < pelletCount; i++)
            {
                float t = pelletCount == 1 ? 0f : (float)i / (pelletCount - 1);
                float angle = baseAngle + Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, t);
                float rad = angle * Mathf.Deg2Rad;
                Vector2 pelletDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                GameObject pellet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                pellet.transform.rotation = Quaternion.Euler(0, 0, angle);

                Rigidbody2D pelletRb = pellet.GetComponent<Rigidbody2D>();
                pelletRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                pelletRb.velocity = pelletDirection * speed;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Dialogue Trigger") || other.gameObject.name.Contains("Bullet") || other.CompareTag("Enemy Boundary"))
                return;

            if (other.CompareTag("Enemy"))
            {
                EnemyScript enemy = other.GetComponent<EnemyScript>();
                if (enemy != null)
                {
                    enemy.Inflict(0.25);
                    HealthScript health = other.GetComponent<HealthScript>();
                    if (health != null)
                        health.EnemyKilled();
                }
                else
                {
                    Debug.Log("No enemy script found on object tagged as enemy");
                }
            }

            if (other.CompareTag("Player"))
            {
                HealthScript health = other.GetComponent<HealthScript>();
                if (health != null)
                    health.Damage();
            }

            Destroy(gameObject);
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}