using UnityEngine;

namespace BulletSquad
{
    public static class GameData
    {
        public static int Score { get; private set; }
        public static int CurrentLevel { get; set; } = 1;

        public static void ResetRun()
        {
            Score = 0;
            CurrentLevel = 1;
        }

        public static void AddScore(int amount)
        {
            Score += Mathf.Max(0, amount);
        }
    }
}
