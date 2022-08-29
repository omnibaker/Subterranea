using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Subterranea
{
    /// <summary>
    /// Button used as link in levels-select section of the Start Menu
    /// </summary>
    public class LevelButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _buttonText = default;
        [SerializeField] private Image _background = default;
        [SerializeField] private Button _button = default;

        /// <summary>
        /// Updates button particulars like text, appearance and behaviours
        /// </summary>
        public void UpdateButtonFeatures(int level, bool isActive, params UnityAction[] actions)
        {
            _buttonText.text = $"LEVEL {level}";

            if (isActive)
            {
                _button.onClick.AddListener(DisableButton);
                foreach (UnityAction action in actions)
                {
                    _button.onClick.AddListener(action);
                }
                _button.interactable = true;
            }
            else
            {
                _background.color = Color.gray;
                _buttonText.color = new Color(_buttonText.color.r, _buttonText.color.g, _buttonText.color.b, 0.4f);
                _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, 0.4f);
                _button.interactable = false;
            }
        }

        /// <summary>
        /// Makes button no longer responsive
        /// </summary>
        public void DisableButton()
        {
            _button.interactable = false;
        }
    }
}