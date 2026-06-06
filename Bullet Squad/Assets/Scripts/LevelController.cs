using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BulletSquad
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private int levelNumber = 1;
        [SerializeField] private string nextSceneName;
        [SerializeField] private Text messageText;

        private bool completed;

        private void Awake()
        {
            GameData.CurrentLevel = levelNumber;
        }

        private void Start()
        {
            if (messageText != null)
            {
                messageText.text = $"MISSION {levelNumber}: CLEAR THE ZONE";
                Invoke(nameof(ClearMessage), 2.5f);
            }
        }

        public void CompleteLevel()
        {
            if (completed)
            {
                return;
            }

            completed = true;
            if (messageText != null)
            {
                messageText.text = levelNumber >= 4 ? "BULLET SQUAD WINS!" : "MISSION COMPLETE";
            }

            Invoke(nameof(LoadNext), 2f);
        }

        private void LoadNext()
        {
            if (string.IsNullOrWhiteSpace(nextSceneName))
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }

        private void ClearMessage()
        {
            if (!completed && messageText != null)
            {
                messageText.text = string.Empty;
            }
        }
    }
}
