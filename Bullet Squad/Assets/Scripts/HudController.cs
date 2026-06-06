using UnityEngine;
using UnityEngine.UI;

namespace BulletSquad
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text healthText;
        [SerializeField] private Text ammoText;
        [SerializeField] private Text grenadeText;
        [SerializeField] private Text objectiveText;
        [SerializeField] private PlayerController player;

        private void Update()
        {
            if (player == null)
            {
                player = FindFirstObjectByType<PlayerController>();
            }

            scoreText.text = $"SCORE {GameData.Score:000000}";
            objectiveText.text = $"MISSION {GameData.CurrentLevel}/4";

            if (player != null)
            {
                healthText.text = $"LIFE {player.Health.Current}/{player.Health.Max}";
                ammoText.text = $"ARMS {player.Ammo:000}";
                grenadeText.text = $"BOMB {player.Grenades:00}";
            }
        }
    }
}
