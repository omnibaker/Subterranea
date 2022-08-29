using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Subterranea
{
    /// <summary>
    /// Controls the root UI elements of the Start Menu UI and calls on the subsections when required
    /// </summary>
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _rootPanel = default;
        [SerializeField] private LevelSelection _levelSelect = default;
        [SerializeField] private Button _startGameButton = default;
        [SerializeField] private TextMeshProUGUI _highestScore = null;

        public void Start()
        {
            AssignButtonActions();
            DisplayRootSection();
            DisplayHighScore();
        }

        /// <summary>
        /// Setup menu button actions
        /// </summary>
        private void AssignButtonActions()
        {
            // TODO: Add click noises to UI
            _startGameButton.onClick.AddListener(DisplayLevelSelect);
            _levelSelect.ReturnButton.onClick.AddListener(DisplayRootSection);
        }

        /// <summary>
        /// Displays root menu section
        /// </summary>
        private void DisplayRootSection()
        {
            _rootPanel.SetActive(true);
            _levelSelect.gameObject.SetActive(false);
        }

        /// <summary>
        /// Displays level select section
        /// </summary>
        private void DisplayLevelSelect()
        {
            _rootPanel.SetActive(false);
            _levelSelect.gameObject.SetActive(true);
            _levelSelect.CreateLevelSelectPanel();
        }

        /// <summary>
        /// Displays highest score from PlayerPref (blank is 0)
        /// </summary>
        private void DisplayHighScore()
        {
            int highScore = PlayerPrefs.GetInt(PrefRef.HIGHEST_SCORE, 0);
            if (highScore > 0)
            {
                _highestScore.text = $"Highest Score: {highScore}";
            }
        }

    }

}