using System;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Bootstrap class that sets up core game components, starts game, then removes itself (needs to run first)
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class GameInit : MonoBehaviour
    {
        // Use this to enable invincibility
        [SerializeField] private bool _godMode = default;

        // Use this for testing if you want to access a higher level (0 refers back to PlayerPrefs)
        [SerializeField] private int _levelUnlock = 0;

        private const string TAG_1 = "Game_T1";
        private const string TAG_2 = "Game_T2";
        private const string TAG_3 = "Game_T3";
        private const string TAG_4 = "Game_T4";
        private string[] _tags = new string[] { TAG_1, TAG_2, TAG_3, TAG_4 };

        private void Awake()
        {
            // Set up editor references
            CreateTagArray();

            // Create level/cave references
            GameData.Levels = new LevelManager().ConstructAllLevels();

            // Update core GameData variables
            GameData.GodMode = _godMode;
            GameData.UnlockLevels = _levelUnlock;
            GameData.IsInitialised = true;
        }
        private void Start()
        {
            // Display Start Menu
            SceneLoader.I.GoToScene(SceneName.STARTMENU);
            Destroy(gameObject);
        }

        /// <summary>
        /// Makes string array or Tag names
        /// </summary>
        public void CreateTagArray()
        {
            int enumCount = Enum.GetNames(typeof(TagNames)).Length;

            for (int i = 0; i < Mathf.Clamp(enumCount, 0, _tags.Length); i++)
            {
                GameTags.Tags.Add(i, _tags[i]);
            }
        }
    }
}