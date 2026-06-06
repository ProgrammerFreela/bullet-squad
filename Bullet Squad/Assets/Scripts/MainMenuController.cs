using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace BulletSquad
{
    public class MainMenuController : MonoBehaviour
    {
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
            {
                StartGame();
            }
            else if (keyboard.digit1Key.wasPressedThisFrame)
            {
                StartLevel1();
            }
            else if (keyboard.digit2Key.wasPressedThisFrame)
            {
                StartLevel2();
            }
            else if (keyboard.digit3Key.wasPressedThisFrame)
            {
                StartLevel3();
            }
            else if (keyboard.digit4Key.wasPressedThisFrame)
            {
                StartLevel4();
            }
        }

        public void StartGame()
        {
            GameData.ResetRun();
            SceneManager.LoadScene("Level01");
        }

        public void OpenLevel(int level)
        {
            GameData.CurrentLevel = level;
            SceneManager.LoadScene($"Level{level:00}");
        }

        public void StartLevel1()
        {
            OpenLevel(1);
        }

        public void StartLevel2()
        {
            OpenLevel(2);
        }

        public void StartLevel3()
        {
            OpenLevel(3);
        }

        public void StartLevel4()
        {
            OpenLevel(4);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
