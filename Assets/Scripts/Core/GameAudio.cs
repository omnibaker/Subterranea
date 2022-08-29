using System.Collections.Generic;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Controller for all game audio (SFX and soundtracks) TODO: Create OST tracks
    /// Concept based on tutorial by Renaissance Coders: https://www.youtube.com/watch?v=3hsBFxrIgQI&t=4430s  
    /// </summary>
    public class GameAudio: Singleton<GameAudio>
    {
        [SerializeField] private GameSFX[] _effects = default;
        [SerializeField] private GameOST[] _soundtracks = default;

        private List<GameObject> _soundObjects = new List<GameObject>();
        private List<Coroutine> _effectCoroutines = new List<Coroutine>();
        private List<Coroutine> _soundtrackCoroutines = new List<Coroutine>();

        private void Awake()
        {
            CreateInstance(this, gameObject);
            InitialiseAudio();
        }

        /// <summary>
        /// Creates audio track instances
        /// </summary>
        private void InitialiseAudio()
        {
            ResetAudioSettings();

            for (int i = 0; i < _effects.Length; i++)
            {
                GameObject go = new GameObject(GetFXName(i));
                _soundObjects.Add(go);
                _effects[i].SetSource(go.AddComponent<AudioSource>());
                _effectCoroutines.Add(null);
            }

            for (int i = 0; i < _soundtracks.Length; i++)
            {
                GameObject go = new GameObject(GetSoundtrackName(i));
                _soundObjects.Add(go);
                _soundtracks[i].SetSource(go.AddComponent<AudioSource>());
                _soundtrackCoroutines.Add(null);
            }
        }

        /// <summary>
        /// Removes all sound object instance
        /// </summary>
        private void ResetAudioSettings()
        {
            for (int i = 0; i < _soundObjects.Count; i++)
            {
                Destroy(_soundObjects[i]);
            }
            _soundObjects.Clear();
        }

        /// <summary>
        /// Returns stylised name for gameObject holding this SFX
        /// </summary>
        private string GetFXName(int index)
        {
            return $"SoundFX_{index}_{System.Enum.GetName(typeof(GameSounds), _effects[index].FX)}";
        }

        /// <summary>
        /// Returns stylised name for gameObject holding this soundtrack
        /// </summary>
        private string GetSoundtrackName(int index)
        {
            return $"SoundOST_{index}_{System.Enum.GetName(typeof(GameMusic), _soundtracks[index].Soundtrack)}";
        }


        /// <summary>
        /// Starts playing given SFX
        /// </summary>
        public void PlayFX(GameSounds audioFX, bool fade = false, float fadeTime = 1f)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                if (_effects[i].FX == audioFX)
                {
                    if (fade)
                    {
                        // Stop any current fading process
                        if (_effectCoroutines[i] != null)
                        {
                            StopCoroutine(_effectCoroutines[i]);
                        }
                        _effectCoroutines[i] = StartCoroutine(_effects[i].PlayWithFade(fadeTime));
                    }
                    else
                    {
                        _effects[i].PlayWithVariance();
                    }

                    return;
                }
            }
            Debug.LogError("No audio sound found on list");
        }

        /// <summary>
        /// Stops given SFX from playing
        /// </summary>
        public void StopFX(GameSounds audioFX, bool fade = false, float fadeTime = 1f)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                if (_effects[i].FX == audioFX)
                {
                    if(fade)
                    {
                        // Stop any current fading process
                        if (_effectCoroutines[i] != null)
                        {
                            StopCoroutine(_effectCoroutines[i]);
                        }
                        _effectCoroutines[i] = StartCoroutine(_effects[i].StopWithFade(fadeTime));
                    }
                    else
                    {
                        _effects[i].Stop();
                    }
                    return;
                }
            }

            Debug.LogError("No audio sound found on list");
        }

        /// <summary>
        /// Starts playing given soundtrack
        /// </summary>
        public void PlaySoundtrack(GameMusic audioSoundtrack)
        {
            for (int i = 0; i < _soundtracks.Length; i++)
            {
                if (_soundtracks[i].Soundtrack == audioSoundtrack)
                {
                    _soundtracks[i].Play();
                    return;
                }
            }

            Debug.LogError("No audio sound found on list");
        }

        /// <summary>
        /// Stops given soundtrack from playing
        /// </summary>
        public void StopSoundtrack(GameMusic audioSoundtrack)
        {
            for (int i = 0; i < _soundtracks.Length; i++)
            {
                if (_soundtracks[i].Soundtrack == audioSoundtrack)
                {
                    _soundtracks[i].Stop();
                    return;
                }
            }

            Debug.LogError("No audio sound found on list");
        }

        /// <summary>
        /// Checks if given sound is currently playing
        /// </summary>
        public bool IsPlaying(GameSounds sfx)
        {
            for (int i = 0; i < _effects.Length; i++)
            {
                if (_effects[i].FX == sfx)
                {
                    return _effects[i].Source.isPlaying;
                }
            }

            return false;
        }
    }


    [System.Serializable]
    public class GameSFX : AudioObject
    {
        public GameSounds FX;
    }


    [System.Serializable]
    public class GameOST : AudioObject
    {
        public GameMusic Soundtrack;
    }
}