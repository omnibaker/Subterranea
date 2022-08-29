using System.Collections;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Floating bonus boxes that player collects to give rewards
    /// </summary>
    public class BonusBox : MonoBehaviour
    {
        [SerializeField] private BonusType _type = BonusType.FullShields;
        [SerializeField] private GameObject _textObject = null;
        [SerializeField] private GameObject _visibleBody = null;

        private Vector3 _textPosition;
        private Vector3 _startPosition;
        private Collider2D _collider;
        private bool _available;

        private GameSounds _bonusSound;
        private enum BonusType { FullShields, OneUp }

        private void Start()
        {
            switch (_type)
            {
                case BonusType.FullShields: _bonusSound = GameSounds.BonusFullShields; break;
                case BonusType.OneUp: _bonusSound = GameSounds.BonusOneUp; break;
            }
        }

        private void Update()
        {
            if (_available)
            {
                OscillateUpAndDown();
            }
        }

        /// <summary>
        /// Smoothly boob up and down in one spot
        /// </summary>
        private void OscillateUpAndDown()
        {
            transform.localPosition = _startPosition + Vector3.up * Mathf.Sin(Time.time * 2f) * 0.1f;
        }

        /// <summary>
        /// Triggered when eligible opject collides with bonus object
        /// </summary>
        public void Collected()
        {
            GameAudio.I.PlayFX(_bonusSound);
            StartCoroutine(DisplayRewardWhenCollected());

            if (_type == BonusType.FullShields)
            {
                Gameplay.I.BonusFullShields();
            }
            else if (_type == BonusType.OneUp)
            {
                Gameplay.I.BonusOneUp();
            }
        }

        /// <summary>
        /// Displays text indicating value of collected bonus object
        /// </summary>
        private IEnumerator DisplayRewardWhenCollected()
        {
            _collider.enabled = false;
            _available = false;
            _visibleBody.SetActive(false);
            _textObject.SetActive(true);

            Vector3 pos = _textObject.transform.localPosition;
            float t = 0;

            while (t < 1f)
            {
                _textObject.transform.localPosition = new Vector3(pos.x, pos.y + t, pos.z);
                t += Time.deltaTime;
                yield return null;
            }

            _textObject.SetActive(false);
        }

        /// <summary>
        /// Displays any object that may have been hidden/collected in a previous attempt of the level
        /// </summary>
        public void ReactivateBonus()
        {
            transform.localPosition = _startPosition;
            _textObject.transform.localPosition = _textPosition;
            _visibleBody.SetActive(true);
            _textObject.SetActive(false);
            _available = true;
            _collider.enabled = true;
        }


        private void OnEnable()
        {
            _startPosition = transform.localPosition;
            _textPosition = _textObject.transform.localPosition;
            _collider = GetComponent<Collider2D>();
        }
    }
}