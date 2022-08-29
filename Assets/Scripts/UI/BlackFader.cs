using System.Collections;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// In-game fader than darkens play area while switching between caves
    /// </summary>
    public class BlackFader : MonoBehaviour
    {
        private const float FADE_SPEED = 2f;
        private CanvasGroup _group;
        private Coroutine _coroutine;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _coroutine = null;
        }

        /// <summary>
        /// Fader is black instantly
        /// </summary>
        public void StraightToBlack()
        {
            _group.alpha = 1f;
        }

        /// <summary>
        /// Cancels any fade already in session, starts new fade to black
        /// </summary>
        public void StartFadeToBlack()
        {
            if (_group.alpha == 1f) return;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _group.blocksRaycasts = true;
            _coroutine = StartCoroutine(Fade(true));
        }

        /// <summary>
        /// Cancels any fade already in session, starts new fade to visible
        /// </summary>
        public void StartFadeToVisible()
        {
            if (_group.alpha == 0f) return;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(Fade(false));
        }

        /// <summary>
        /// Fades in/out at rate of FADE_SPEED constant
        /// </summary>
        private IEnumerator Fade(bool fadeToBlack)
        {
            float t, start, end;

            if (fadeToBlack)
            {
                t = _group.alpha;
                start = 0;
                end = 1;
            }
            else
            {
                t = 1f - _group.alpha;
                start = 1;
                end = 0;
            }

            while (t < 1f)
            {
                _group.alpha = Mathf.Lerp(start, end, t);
                t += Time.deltaTime * FADE_SPEED;
                yield return null;
            }

            _group.alpha = end;
            _group.blocksRaycasts = fadeToBlack;
        }
    }
}
