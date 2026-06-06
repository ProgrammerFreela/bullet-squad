using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace BulletSquad
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float jumpForce = 13f;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject grenadePrefab;
        [SerializeField] private LayerMask enemyLayers;
        [SerializeField] private float fireCooldown = 0.14f;
        [SerializeField] private int maxAmmo = 120;
        [SerializeField] private int grenades = 6;

        private Rigidbody2D body;
        private Health health;
        private float horizontal;
        private bool grounded;
        private int facing = 1;
        private int ammo;
        private float nextFireTime;

        public int Ammo => ammo;
        public int Grenades => grenades;
        public Health Health => health;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            ammo = maxAmmo;
            health.died.AddListener(OnDied);
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (keyboard == null)
            {
                return;
            }

            horizontal = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                horizontal -= 1f;
            }
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                horizontal += 1f;
            }

            if (horizontal != 0)
            {
                facing = horizontal > 0 ? 1 : -1;
                transform.localScale = new Vector3(facing, 1f, 1f);
            }

            bool jumpPressed = keyboard.spaceKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame;
            if (jumpPressed && grounded)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            }

            bool shootPressed = keyboard.jKey.isPressed || keyboard.leftCtrlKey.isPressed || (mouse != null && mouse.leftButton.isPressed);
            if (shootPressed && Time.time >= nextFireTime)
            {
                Shoot();
            }

            bool grenadePressed = keyboard.kKey.wasPressedThisFrame || (mouse != null && mouse.rightButton.wasPressedThisFrame);
            if (grenadePressed && grenades > 0)
            {
                ThrowGrenade();
            }
        }

        private void FixedUpdate()
        {
            body.linearVelocity = new Vector2(horizontal * moveSpeed, body.linearVelocity.y);
        }

        private void Shoot()
        {
            if (ammo <= 0 || bulletPrefab == null)
            {
                return;
            }

            nextFireTime = Time.time + fireCooldown;
            ammo--;
            GameObject shot = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            shot.GetComponent<Projectile>().Launch(Vector2.right * facing, 1, 15f, enemyLayers, false);
        }

        private void ThrowGrenade()
        {
            if (grenadePrefab == null)
            {
                return;
            }

            grenades--;
            GameObject grenade = Instantiate(grenadePrefab, firePoint.position + Vector3.up * 0.2f, Quaternion.identity);
            grenade.GetComponent<Projectile>().Launch(new Vector2(facing, 0.35f), 3, 7f, enemyLayers, true);
        }

        public void AddAmmo(int amount)
        {
            ammo = Mathf.Min(maxAmmo, ammo + amount);
        }

        public void AddGrenades(int amount)
        {
            grenades += amount;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    grounded = true;
                    return;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            grounded = false;
        }

        private void OnDied()
        {
            Invoke(nameof(RestartLevel), 1.2f);
        }

        private void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
