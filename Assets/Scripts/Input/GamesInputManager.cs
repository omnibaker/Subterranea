using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Subterranea
{
    public class GamesInputManager : MonoBehaviour
    {
        public bool ThrustLocked { get; set; }

        public delegate void DPadMove(Vector2 moveDirection);
        public event DPadMove OnDPadMoveEvent;

        public delegate void ThrustPressed(bool state);
        public event ThrustPressed OnThrustPressedEvent;

        [Header("Button Images")]
        [SerializeField] private Image _leftThrusterImg = null;
        [SerializeField] private Image _rightThrusterImg = null;
        [SerializeField] private Image _thrustButtonImg = null;

        [Header("Activation Colors")]
        [SerializeField] private Color _colorTouched = new Color(0f, 0.514f, .954f, 1f); //0083F3
        [SerializeField] private Color _colorUntouched = new Color(0.549f, 0.549f, 0.549f, 1f); //8C8C8C
        //[SerializeField] private Color _colorDisabled = new Color(0.9f, 0.9f, 0.9f, 1f);

        private GamesInputActions _gia;
        //private bool _thrustDirectionCurrentShowing;

        private void Awake()
        {
            _gia = new GamesInputActions();
        }
        private void Update()
        {
            RespondToLeftDirectionInput();
        }

        /// <summary>
        /// Desktop users only need keyboard, on-screen controller buttons removed
        /// </summary>
        public void RemoveMobileInputs()
        {
            foreach (Transform tf in transform)
            {
                tf.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Triggered when boost/thrust button is pressed down
        /// </summary>
        private void StartBoost(InputAction.CallbackContext obj)
        {
            if (!ThrustLocked)
            {
                OnThrustPressedEvent?.Invoke(true);
            }
        }

        /// <summary>
        /// Triggered when boost/thrust button is released
        /// </summary>
        private void StopBoost(InputAction.CallbackContext obj)
        {
            if (!ThrustLocked)
            {
                OnThrustPressedEvent?.Invoke(false);
            }
        }

        /// <summary>
        /// Update latest state of d-pad movement, return Vector3.zero when neither left/right pressed
        /// </summary>
        private void RespondToLeftDirectionInput()
        {
            OnDPadMoveEvent?.Invoke(_gia.Player.Rotate.ReadValue<Vector2>());
        }

        /// <summary>
        /// UI colour-change responses to boost/thrust being pressed/released
        /// </summary>
        public void DisplayThrustButtonUsage(bool pressed)
        {
            _thrustButtonImg.color = pressed ? _colorTouched : _colorUntouched;
        }

        /// <summary>
        /// UI colour-change responses to left/right rotation being pressed/released
        /// </summary>
        public void DisplayTurnButtonUsage(bool anyButtonPressed, bool isLeft = true)
        {
            if (anyButtonPressed)
            {
                //_thrustDirectionCurrentShowing = true;
                if (isLeft)
                {
                    _leftThrusterImg.color = _colorTouched;
                    _rightThrusterImg.color = _colorUntouched;
                }
                else
                {
                    _leftThrusterImg.color = _colorUntouched;
                    _rightThrusterImg.color = _colorTouched;
                }
            }
            else
            {
                //if (_thrustDirectionCurrentShowing)
                //{
                //    //_thrustDirectionCurrentShowing = false;
                //    _leftThrusterImg.color = _colorUntouched;
                //    _rightThrusterImg.color = _colorUntouched;
                //}
                _leftThrusterImg.color = _colorUntouched;
                _rightThrusterImg.color = _colorUntouched;
            }
        }

        private void OnEnable()
        {
            _gia.Player.Enable();
            _gia.Player.MainThrust.performed += StartBoost;
            _gia.Player.MainThrust.canceled += StopBoost;
        }
        private void OnDisable()
        {
            _gia.Player.MainThrust.performed -= StartBoost;
            _gia.Player.MainThrust.canceled -= StopBoost;
            _gia.Player.Disable();
        }
    }
}