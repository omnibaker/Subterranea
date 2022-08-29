using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Subterranea
{
    /// <summary>
    /// Manages the display of all gameplay HUD area texts and also controls pop-ups windows
    /// </summary>
    public class PlayUI : MonoBehaviour
    {
        [SerializeField] private Button _exitSession = null;

        [Header("Pop-Up Prefabs")]
        [SerializeField] private GameObject _optionsPopUpPF;
        [SerializeField] private GameObject _bonusPointsPopUpPF;

        [Header("On-Screen Text Fields")]
        [SerializeField] private TextMeshProUGUI _remainingTime = null;
        [SerializeField] private TextMeshProUGUI _score = null;
        [SerializeField] private TextMeshProUGUI _level = null;
        [SerializeField] private TextMeshProUGUI _livesCount = null;
        [SerializeField] private TextMeshProUGUI _messageField = null;


        private Coroutine _dynamicMessageWaiter;
        private const int ZERO_SCORE = 0;
        private const string ZERO_TIME = "00:00";
        //private const string SCORE = "Score:\t {0}";
        //private const string TIME = "Time:\t {0}";
        private const string CAVE = "Cave:\t {0}";

        private void Awake()
        {
            AssignButtonBehaviour();
        }

        /// <summary>
        /// Initialises buttons by adding actions
        /// </summary>
        private void AssignButtonBehaviour()
        {
            _exitSession.onClick.AddListener(() => CreateOptionsPopUp(OptionPanelType.GameQuit));
        }

        /// <summary>
        /// Updates current score displayed in HUD
        /// </summary>
        public void UpdateScore(int score)
        {
            //_score.text = string.Format(SCORE, score);
            _score.text = score.ToString();
        }

        /// <summary>
        /// Updates remaining time displayed in HUD
        /// </summary>
        public void UpdateRemainingTime(string timeTaken = "")
        {
            //_remainingTime.text = string.Format(TIME, timeTaken);
            _remainingTime.text = timeTaken;
        }

        /// <summary>
        /// Reset life count displayed in HUD to initial value
        /// </summary>
        public void ResetLives()
        {
            _livesCount.text = GameData.START_LIFE_TOTAL.ToString();
        }

        /// <summary>
        /// Reset life count displayed in HUD to initial value
        /// </summary>
        public void UpdateLevelDisplay()
        {
            _level.text = string.Format(CAVE, $"{GameData.CurrentLevel}-{GameData.CurrentCave}");
        }

        /// <summary>
        /// Resets HUD display values
        /// </summary>
        public void ResetUI()
        {
            // TODO: Should these be done in accordance to data? i.e. display != data
            ResetLives();
            UpdateScore(ZERO_SCORE);
            UpdateRemainingTime(ZERO_TIME);
        }

        /// <summary>
        /// Update life count displayed in HUD to initial value
        /// </summary>
        public void UpdateLives(int lives)
        {
            _livesCount.text = lives.ToString();
        }

        /// <summary>
        /// Creates a pop-up window with two options to choose from
        /// </summary>
        public void CreateOptionsPopUp(OptionPanelType option)
        {
            // Create object from prefab and bring in forward
            GameObject popUp = Instantiate(_optionsPopUpPF, transform);
            popUp.transform.SetAsLastSibling();

            // Use Options component to create appropriate pop-up window
            OptionsPopUp options = popUp.GetComponent<OptionsPopUp>();
            switch (option)
            {
                case OptionPanelType.GameCompleted: options.GameCompletedDisplay(); break;
                case OptionPanelType.GameOver: options.GameOverDisplay(); break;
                case OptionPanelType.GameQuit: options.GameQuitDisplay(); break;
            }
        }

        /// <summary>
        /// Creates a pop-up window for when calculating Bonus Points at end of each cave
        /// </summary>
        public IEnumerator CreateBonusPointsPopUp()
        {
            // Create object from prefab and bring in forward
            GameObject bonusPoints = Instantiate(_bonusPointsPopUpPF, transform);
            bonusPoints.transform.SetAsLastSibling();

            // Use BonusPoints component to create the bonus points module
            yield return StartCoroutine(bonusPoints.GetComponent<BonusPointsPopUp>().CalculateBonusPoints(UpdateScore));
        }            

        /// <summary>
        /// Clear the pop-up window container of any existing pop-ups
        /// </summary>
        public void RemoveAllPopUps()
        {
            foreach (OptionsPopUp options in FindObjectsOfType<OptionsPopUp>())
            {
                options.Close();
            }
            foreach (BonusPointsPopUp bonusPoints in FindObjectsOfType<BonusPointsPopUp>())
            {
                bonusPoints.Close();
            }
        }

        /// <summary>
        /// Stop any previous message process and clear message
        /// </summary>
        public void ClearMessage()
        {
            if (_dynamicMessageWaiter != null)
            {
                StopCoroutine(_dynamicMessageWaiter);
            }
            _dynamicMessageWaiter = StartCoroutine(DynamicMessage("", 0));
        }


        /// <summary>
        /// Stop any previous message process and start new message
        /// </summary>
        public void UpdateMessage(string msg = "", float waitLenght = UIValues.DYNAMIC_WAIT)
        {
            if (_dynamicMessageWaiter != null)
            {
                StopCoroutine(_dynamicMessageWaiter);
            }
            _dynamicMessageWaiter = StartCoroutine(DynamicMessage(msg, waitLenght));
        }

        /// <summary>
        /// Runs message process that stays visible for set amount of time
        /// </summary>
        public IEnumerator DynamicMessage(string msg, float waitLenght)
        {
            // Activate if not already done so
            if(!_messageField.gameObject.activeInHierarchy)
            {
                _messageField.gameObject.SetActive(true);
            }
            
            // Update and wait
            _messageField.text = msg;
            yield return new WaitForSeconds(waitLenght);

            // Clear and remove
            _messageField.text = "";
            _messageField.gameObject.SetActive(false);
        }




    }
}