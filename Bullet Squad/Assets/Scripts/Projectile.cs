using UnityEngine;

namespace BulletSquad
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float speed = 12f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool explosive;
        [SerializeField] private float explosionRadius = 1.4f;

        private Vector2 direction = Vector2.right;

        public void Launch(Vector2 launchDirection, int projectileDamage, float projectileSpeed, LayerMask targets, bool isExplosive)
        {
            direction = launchDirection.normalized;
            damage = projectileDamage;
            speed = projectileSpeed;
            targetLayers = targets;
            explosive = isExplosive;
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & targetLayers.value) == 0)
            {
                return;
            }

            if (explosive)
            {
                foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayers))
                {
                    hit.GetComponent<Health>()?.Damage(damage);
                }
            }
            else
            {
                other.GetComponent<Health>()?.Damage(damage);
            }

            Destroy(gameObject);
        }
    }
}
