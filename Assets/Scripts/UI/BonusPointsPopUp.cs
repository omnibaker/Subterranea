using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Subterranea
{
    /// <summary>
    /// Controller for bonus points tally component that displays when completing a cave
    /// </summary>
    public class BonusPointsPopUp : MonoBehaviour
    {
        [SerializeField] private GameObject _visible = null;

        [Header("Text Fields")]
        [SerializeField] private TextMeshProUGUI _bonusTime = null;
        [SerializeField] private TextMeshProUGUI _bonusPoint = null;

        /// <summary>
        /// Updates text field for both time and points
        /// </summary>
        public void UpdateBonusMessage(string timeRemaining = "", int points = 0)
        {
            if (string.IsNullOrEmpty(timeRemaining))
            {
                _bonusTime.text = "00:00";
                _bonusPoint.text = "0";
            }
            else
            {
                _bonusTime.text = timeRemaining;
                _bonusPoint.text = points.ToString();
            }
        }
       
        /// <summary>
        /// Cycles through remaining seconds of time remaining, increasing the amount of bonus points rewarded
        /// </summary>
        public IEnumerator CalculateBonusPoints(UnityAction<int> updateScore)
        {
            yield return new WaitForSeconds(1f);

            // Display initial values before counting
            UpdateBonusMessage(GameUtils.GetTimeInFormattedString(GameData.TimeRemaining), 0);
            updateScore.Invoke(GameData.CurrentScore);
            _visible.SetActive(true);

            yield return new WaitForSeconds(1f);

            // Cycle through remaing time, awarding points for each second
            int points = 0;
            int score = GameData.CurrentScore;
            while (GameData.TimeRemaining >= 1)
            {
                GameData.TimeRemaining--;
                points += 5;
                UpdateBonusMessage(GameUtils.GetTimeInFormattedString(GameData.TimeRemaining), points);
                updateScore.Invoke(score + points);

                // Slight delay to display changing values over short amount of time
                yield return new WaitForSeconds(0.02f);
            }

            // Final display and unregister all event handlers 
            GameData.CurrentScore += points;
            GameData.CheckHighestScore();
            updateScore.Invoke(GameData.CurrentScore);

            yield return new WaitForSeconds(1f);
            Close();
        }

        /// <summary>
        /// Removes bonus points section
        /// </summary>
        public void Close()
        {
            Destroy(gameObject);
        }
    }

}