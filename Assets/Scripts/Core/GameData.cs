using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Static class that holds all runtime game data
    /// </summary>
    public static class GameData
    {
        public const int START_LIFE_TOTAL = 3;
        public static bool GodMode { get; set; }
        public static CaveManager CurrentCaveManager { get; set; }
        public static int CurrentLevel { get; set; } = 1;
        public static int SelectedLevel { get; set; } = 1;
        public static int CurrentScore { get; set; } = 0;
        public static int CurrentLives { get; set; } = START_LIFE_TOTAL;
        public static int CurrentCave { get; set; } = 1;
        public static float RewardMultiple { get; set; } = 1f;
        public static float TimeRemaining { get; set; }
        public static int UnlockLevels { get; set; }
        public static bool IsInitialised { get; set; }
        public static Dictionary<int, List<GameObject>> Levels { get; set; } = new Dictionary<int, List<GameObject>>();

        /// <summary>
        /// Update highest score
        /// </summary>
        public static void CheckHighestScore()
        {
            if (CurrentScore > PlayerPrefs.GetInt(PrefRef.HIGHEST_SCORE, 0))
            {
                PlayerPrefs.SetInt(PrefRef.HIGHEST_SCORE, CurrentScore);
            }
        }

        /// <summary>
        /// Get the highest level that has been unlocked by user
        /// </summary>
        public static int GetHighestUnlockedLevel()
        {
            if (UnlockLevels > 0)
            {
                return UnlockLevels;
            }
            else
            {
                return PlayerPrefs.GetInt(PrefRef.HIGHEST_LEVEL, 1);
            }
        }

        /// <summary>
        /// Resets values for new level 
        /// </summary>
        public static void SetCurrentLevelAndCave(int level)
        {
            SelectedLevel = level;
            CurrentLevel = level;
            CurrentCave = 1;
        }

        /// <summary>
        /// When exiting gameplay (death/quit/over) checks if any values need to be saved
        /// </summary>
        public static void CheckData()
        {
            // TODO: Should check scores to see if milestones, reset data maybe on a boolean?
        }

        /// <summary>
        /// Resets all GameData values, should be done in tandem with HUD values
        /// </summary>
        public static void ResetLevelsAndScores(bool hardReset = false)
        {
            CurrentLevel = hardReset ? 1 : SelectedLevel;
            SelectedLevel = hardReset ? 1 : SelectedLevel;
            CurrentScore = 0;
            CurrentLives = START_LIFE_TOTAL;
            CurrentCave = 1;
        }
    }
}