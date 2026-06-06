using UnityEngine;

namespace BulletSquad
{
    [RequireComponent(typeof(Health))]
    public class BossController : MonoBehaviour
    {
        [SerializeField] private Transform[] firePoints;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private LayerMask playerLayers;
        [SerializeField] private float fireCooldown = 0.55f;
        [SerializeField] private int scoreValue = 1000;

        private float nextFireTime;
        private Transform player;
        private LevelController level;

        private void Awake()
        {
            GetComponent<Health>().died.AddListener(OnDied);
        }

        private void Start()
        {
            player = FindFirstObjectByType<PlayerController>()?.transform;
            level = FindFirstObjectByType<LevelController>();
        }

        private void Update()
        {
            if (player == null || Time.time < nextFireTime)
            {
                return;
            }

            nextFireTime = Time.time + fireCooldown;
            foreach (Transform point in firePoints)
            {
                Vector2 direction = ((Vector2)player.position - (Vector2)point.position).normalized;
                GameObject shot = Instantiate(projectilePrefab, point.position, Quaternion.identity);
                shot.GetComponent<Projectile>().Launch(direction, 1, 6f, playerLayers, false);
            }
        }

        private void OnDied()
        {
            GameData.AddScore(scoreValue);
            level?.CompleteLevel();
            Destroy(gameObject);
        }
    }
}
