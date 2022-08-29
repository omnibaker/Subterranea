using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Subterranea
{
    /// <summary>
    /// Controls the Level Selection section of the Start Menu
    /// </summary>
    public class LevelSelection : MonoBehaviour
    {
        [SerializeField] private GameObject _levelButtonPF = default;
        [SerializeField] private Transform _levelButtonContainer = default;
        [SerializeField] private Button _returnButton = default;

        public Button ReturnButton { get { return _returnButton; } }

        private List<LevelButton> _levelButtons = new List<LevelButton>();

        /// <summary>
        /// Creates buttons in container for each available level
        /// </summary>
        public void CreateLevelSelectPanel()
        {
            if (_levelButtons.Count == 0)
            {
                for (int i = 1; i <= GameData.Levels.Count; i++)
                {
                    GameObject button = Instantiate(_levelButtonPF, _levelButtonContainer);
                    if (i <= GameData.GetHighestUnlockedLevel())
                    {
                        int level = i;
                        void setLevelCave() => GameData.SetCurrentLevelAndCave(level);
                        void startNewGame() => SceneLoader.I.GoToScene(SceneName.GAMEPLAY);
                        LevelButton levelButton = button.GetComponent<LevelButton>();
                        levelButton.UpdateButtonFeatures(level, true, setLevelCave, startNewGame);
                        _levelButtons.Add(levelButton);
                    }
                    else
                    {
                        button.GetComponent<LevelButton>().UpdateButtonFeatures(i, false);
                    }
                }
            }
        }
    }
}