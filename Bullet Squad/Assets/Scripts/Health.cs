using UnityEngine;
using UnityEngine.Events;

namespace BulletSquad
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float invulnerabilityTime = 0.2f;

        public UnityEvent<int, int> changed = new UnityEvent<int, int>();
        public UnityEvent died = new UnityEvent();

        private int currentHealth;
        private float nextDamageTime;

        public int Current => currentHealth;
        public int Max => maxHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Start()
        {
            changed.Invoke(currentHealth, maxHealth);
        }

        public void Damage(int amount)
        {
            if (amount <= 0 || Time.time < nextDamageTime || currentHealth <= 0)
            {
                return;
            }

            nextDamageTime = Time.time + invulnerabilityTime;
            currentHealth = Mathf.Max(0, currentHealth - amount);
            changed.Invoke(currentHealth, maxHealth);

            if (currentHealth == 0)
            {
                died.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || currentHealth <= 0)
            {
                return;
            }

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            changed.Invoke(currentHealth, maxHealth);
        }
    }
}
