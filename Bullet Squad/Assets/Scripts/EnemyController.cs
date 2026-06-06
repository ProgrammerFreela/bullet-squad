using UnityEngine;

namespace BulletSquad
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float detectionRange = 9f;
        [SerializeField] private float stopDistance = 4f;
        [SerializeField] private float fireCooldown = 1.2f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private int scoreValue = 100;

        private Rigidbody2D body;
        private Transform player;
        private float nextFireTime;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            GetComponent<Health>().died.AddListener(OnDied);
        }

        private void Start()
        {
            PlayerController found = FindFirstObjectByType<PlayerController>();
            player = found != null ? found.transform : null;
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            float distance = Vector2.Distance(transform.position, player.position);
            int direction = player.position.x >= transform.position.x ? 1 : -1;
            transform.localScale = new Vector3(direction, 1f, 1f);

            if (distance < detectionRange && distance > stopDistance)
            {
                body.linearVelocity = new Vector2(direction * moveSpeed, body.linearVelocity.y);
            }
            else
            {
                body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            }

            if (distance < detectionRange && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireCooldown;
                GameObject shot = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                shot.GetComponent<Projectile>().Launch(Vector2.right * direction, 1, 8f, playerLayers, false);
            }
        }

        private void OnDied()
        {
            GameData.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
