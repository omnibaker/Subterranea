using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Subterranea
{
    /// <summary>
    /// Controls the UI components that appears when system requires an answer to a yes/no question 
    /// </summary>
    public class OptionsPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject _visible = null;

        [Header("Buttons")]
        [SerializeField] private Button _leftButton = null;
        [SerializeField] private Button _rightButton = null;

        [Header("Text Fields")]
        [SerializeField] private TextMeshProUGUI _labelTMP = null;
        [SerializeField] private TextMeshProUGUI _messageTMP = null;
        [SerializeField] private TextMeshProUGUI _leftTMP = null;
        [SerializeField] private TextMeshProUGUI _rightTMP = null;

        /// <summary>
        /// Updates pop-up action for left/primary button
        /// </summary>
        public void UpdateLeftButton(UnityAction leftAction)
        {
            _leftButton.onClick.AddListener(leftAction);
        }

        /// <summary>
        /// Updates pop-up action for right button
        /// </summary>
        public void UpdateRightButton(UnityAction rightAction)
        {
            _rightButton.onClick.AddListener(rightAction);
        }

        /// <summary>
        /// Updates text for popup header and message
        /// </summary>
        public void UpdateHeader(string label, string msg)
        {
            if (_labelTMP != null) _labelTMP.text = label;
            if (_messageTMP != null) _messageTMP.text = msg;
        }

        /// <summary>
        /// Updates text for both popup buttons
        /// </summary>
        public void UpdateButtonText(string left, string right)
        {
            if (_leftTMP != null) _leftTMP.text = left;
            if (_rightTMP != null) _rightTMP.text = right;
        }

        /// <summary>
        /// Updates text for popup button when only one is needed
        /// </summary>
        public void SingleAnswerOnly(string singleReponseLabel)
        {
            UpdateButtonText(singleReponseLabel, "");
            _rightButton.gameObject.SetActive(false);
        }


        /// <summary>
        /// Removes bonus points section
        /// </summary>
        public void Close()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Make each button close options window
        /// </summary>
        public void AddClosers()
        {
            UpdateLeftButton(Close);
            UpdateRightButton(Close);
        }

        /// <summary>
        /// Options display when game is over, i.e. player has ran out of lives
        /// </summary>
        public void GameOverDisplay()
        {
            UpdateHeader(UIHeader.GAME_OVER, UIMessages.PLAY_AGAIN);
            UpdateButtonText(UIButtonLabels.YES, UIButtonLabels.NO);
            UpdateLeftButton(Gameplay.I.RefreshForNewGame);
            UpdateLeftButton(() => Gameplay.I.StartNewGame());
            UpdateRightButton(Gameplay.I.QuitPlay);
            AddClosers();

            _visible.SetActive(true);
        }

        /// <summary>
        /// Options display when game has been quit by user prematurely
        /// </summary>
        public void GameQuitDisplay()
        {
            UpdateHeader(UIHeader.GAME_QUIT, UIMessages.QUIT_GAME);
            UpdateButtonText(UIButtonLabels.YES, UIButtonLabels.NO);
            UpdateLeftButton(Gameplay.I.QuitPlay);
            UpdateLeftButton(GameData.CheckData);
            UpdateRightButton(() => Gameplay.I.UnpauseGame());
            AddClosers();

            _visible.SetActive(true);
        }

        /// <summary>
        /// Options display when game has been completed
        /// </summary>
        public void GameCompletedDisplay()
        {
            UpdateHeader(UIHeader.GAME_COMPLETED, UIMessages.COMPLETED_GAME);
            SingleAnswerOnly(UIButtonLabels.OK);
            UpdateLeftButton(Gameplay.I.QuitPlay);
            UpdateLeftButton(GameData.CheckData);
            AddClosers();

            _visible.SetActive(true);
        }
    }
}