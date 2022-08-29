using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Adjusts play area size (camera distance) to accomodate device screen
    /// </summary>
    public class BorderControl : MonoBehaviour
    {
        [SerializeField] private float _camSizeAt2_1 = 3;

        private Camera _orthographicCamera = null;
        private Vector2 _screenSize;

        private void Awake()
        {
            if (_orthographicCamera == null)
            {
                _orthographicCamera = Camera.main;
            }

            _orthographicCamera.orthographicSize = UpdateBorders();
        }

        /// <summary>
        /// Calculates best zoom for device's screen ratio
        /// </summary>
        private float UpdateBorders()
        {
            _screenSize = new Vector2(Screen.width, Screen.height);
            Debug.Log($"Screen Dimensions: {_screenSize.x} x {_screenSize.y}");

            float ratio = _screenSize.y / _screenSize.x;
            float factor = 2f * _camSizeAt2_1; // 
            float newSize = ratio * factor; //10 @ def5
            Debug.Log($"Orthographic Camera Size: {GameUtils.FormatFloats(newSize, 3)}");

            return newSize;
        }
    }
}
