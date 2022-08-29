using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Controls the platform objects that player start/finishes on
    /// </summary>
    public class PlatformControl : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _lights;
        [SerializeField] private float _speed = 2;
        [SerializeField] private Color _lightColor = Color.red;
        private Color _fadedLightColor;

        private float _time;
        private Color _startColor;
        private Color _endColor;
        private bool _fadingIn;

        private void Awake()
        {
            _fadedLightColor = new Color(_lightColor.r, _lightColor.g, _lightColor.b, 0);
        }
        private void Start()
        {
            SwitchColorDirection();
        }

        private void Update()
        {
            LightFade();
        }

        /// <summary>
        /// Fades platform light
        /// </summary>
        private void LightFade()
        {
            if (_time > 1f)
            {
                SwitchColorDirection();
            }

            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].color = Color.Lerp(_startColor, _endColor, _time);
            }

            _time += Time.deltaTime * _speed;
        }

        /// <summary>
        /// Determines direction of fade (i.e. 0 -> 1 or  1 -> 0)
        /// </summary>
        private void SwitchColorDirection()
        {
            if (_fadingIn)
            {
                _startColor = _fadedLightColor;
                _endColor = _lightColor;
            }
            else
            {
                _startColor = _lightColor;
                _endColor = _fadedLightColor;
            }

            _fadingIn = !_fadingIn;
            _time = 0;
        }
    }
}
