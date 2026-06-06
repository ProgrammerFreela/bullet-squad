using UnityEngine;

namespace BulletSquad
{
    public class Pickup : MonoBehaviour
    {
        public enum PickupType
        {
            Health,
            Ammo,
            Grenade
        }

        [SerializeField] private PickupType type;
        [SerializeField] private int amount = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player == null)
            {
                return;
            }

            switch (type)
            {
                case PickupType.Health:
                    player.Health.Heal(amount);
                    break;
                case PickupType.Ammo:
                    player.AddAmmo(amount);
                    break;
                case PickupType.Grenade:
                    player.AddGrenades(amount);
                    break;
            }

            Destroy(gameObject);
        }
    }
}
