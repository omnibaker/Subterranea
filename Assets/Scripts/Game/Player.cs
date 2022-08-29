using System.Collections;
using UnityEngine;

namespace Subterranea
{
    /// <summary>
    /// Controls all behaviours of main user/player
    /// </summary>
    public class Player : MonoBehaviour
    {
        [Header("Adjustable Values")]
        [SerializeField] private float _mainThrust = 400f;
        [SerializeField] private float _rotationThrust = 200f;
        [SerializeField] private float _thrustMax = 2.5f;
        [SerializeField] private float _gravityScale = 0.3f;

        [Header("Action Objects")]
        [SerializeField] private GameObject _explosion;
        [SerializeField] private ParticleSystem _engineParticleSystem = null;

        [Header("Player Sprite Renderers")]
        [SerializeField] private SpriteRenderer _playerState;
        [SerializeField] private SpriteRenderer _damageState;

        //TODO: Create craft/shield images that show deteriorating hull
        [Header("Health/State")]
        [SerializeField] private Sprite[] _damageSprites;
        [SerializeField] private Sprite[] ShieldSprites;
        [SerializeField] private Color[] ShieldColors;

        private const int MAX_SHIELD_DAMAGE = 5;
        private const float LANDING_HOLD_TIME = 1.5f;
        private const float THRUSTER_FADE = 0.15f;
        private const float INPUT_ACCELRERATION_FACTOR = 3f;
        private const string HORIZONTAL = "Horizontal";

        private Rigidbody2D _rigidBody;
        private PolygonCollider2D _collider;
        private Coroutine _hitFading;
        private Animator _animator;
        private Vector3 _pausedVelocity = Vector3.zero;

        private float _timeOnLandingPad;
        private float _rotationalDpadValue;
        private float _rotationalValue;
        private float _maxShieldDamage = MAX_SHIELD_DAMAGE;
        private bool _thrustingHeld;
        private int _shieldDamage;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _collider = GetComponent<PolygonCollider2D>();
        }
   
        private void Update()
        {
            string particle = _engineParticleSystem != null ? _engineParticleSystem.ToString() : "NONE";

            if (!Gameplay.I.IsPaused)
            {
                ProcessThrust();
#if UNITY_EDITOR 
                ProcessAxisViaLegacyInput();
                ProcessAxisViaNewInputSystem();
#elif UNITY_STANDALONE
                ProcessAxisViaLegacyInput();
#elif UNITY_IOS || UNITY_ANDROID
                ProcessAxisViaNewInputSystem();
#endif
            }
        }

        /// <summary>
        /// Base function when main action button is pressed/released (true/false)
        /// </summary>
        private void ThrustPressed(bool hasBeenPressed)
        {
            Gameplay.I.InputManager.DisplayThrustButtonUsage(hasBeenPressed);
            _thrustingHeld = hasBeenPressed;
            if (!_thrustingHeld)
            {
                DisableThrustEffects();
            }
        }

        /// <summary>
        /// Base function when left joystick/dpad is moved
        /// </summary>
        private void DPadActivated(Vector2 axisDirection)
        {
            if (!Gameplay.I.IsPaused)
            {
                _rotationalDpadValue = axisDirection.x;
            }
            else
            {
                _rotationalDpadValue = 0;
            }
        }

        /// <summary>
        /// Used for toggling visibility between deaths/levels
        /// </summary>
        public void Activate(bool activate)
        {
            if (activate)
            {
                _damageState.gameObject.SetActive(false);
                _explosion.gameObject.SetActive(false);
                _engineParticleSystem.gameObject.SetActive(true);
            }
            else
            {
                _damageState.gameObject.SetActive(false);
                _explosion.gameObject.SetActive(false);
                _engineParticleSystem.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Check if thrusting button pressed, starts/stops accordingly
        /// </summary>
        private void ProcessThrust()
        {
            if (_thrustingHeld || Input.GetKey(KeyCode.Space))
            {
                StartThrusting();
            }
            else
            {
                DisableThrustEffects();
            }
        }

        /// <summary>
        /// Get axis data from Unity's legacy input system
        /// </summary>
        private void ProcessAxisViaLegacyInput()
        {
            if (Input.GetAxis(HORIZONTAL) != 0)
            {
                ApplyRotation(Input.GetAxis(HORIZONTAL));
            }
        }

        /// <summary>
        /// Get axis data from Unity's legacy input system
        /// </summary>
        private void ProcessAxisViaNewInputSystem()
        {
            if (_rotationalDpadValue != 0)
            {
                if (_rotationalDpadValue > 0)
                {
                    //Gameplay.I.InputManager.DisplayTurnButtonUsage(true, false);
                    _rotationalValue += INPUT_ACCELRERATION_FACTOR * Time.deltaTime; ;
                }
                else if (_rotationalDpadValue < 0)
                {
                    //Gameplay.I.InputManager.DisplayTurnButtonUsage(true, true);
                    _rotationalValue -= INPUT_ACCELRERATION_FACTOR * Time.deltaTime; ;
                }
                _rotationalValue = Mathf.Clamp(_rotationalValue, -1f, 1f);
                ApplyRotation(_rotationalValue);
            }
            else
            {
#if UNITY_EDITOR
                if(Input.GetAxis(HORIZONTAL) == 0)
                {
                    Gameplay.I.InputManager.DisplayTurnButtonUsage(false);
                }
#else
                Gameplay.I.InputManager.DisplayTurnButtonUsage(false);
#endif
                if (Gameplay.I.IsPaused)
                {
                    _rotationalValue = 0;
                }
                else
                {
                    if (_rotationalValue < 0)
                    {
                        float newRotationalValue = _rotationalValue + INPUT_ACCELRERATION_FACTOR * Time.deltaTime;
                        if (newRotationalValue > 0)
                        {
                            _rotationalValue = 0;
                        }
                        else
                        {
                            _rotationalValue += INPUT_ACCELRERATION_FACTOR * Time.deltaTime;
                        }

                    }
                    else if (_rotationalValue > 0)
                    {
                        float newRotationalValue = _rotationalValue - INPUT_ACCELRERATION_FACTOR * Time.deltaTime;
                        if (newRotationalValue < 0)
                        {
                            _rotationalValue = 0;
                        }
                        else
                        {
                            _rotationalValue -= INPUT_ACCELRERATION_FACTOR * Time.deltaTime;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Push player forward using Unity physics system force
        /// </summary>
        private void StartThrusting()
        {
            if (!Gameplay.I.IsPaused)
            {
                if (_engineParticleSystem.isStopped)
                {
                    _engineParticleSystem.Play();
                }
                Vector3 thrust = Vector3.up * _mainThrust * Time.deltaTime;
                _rigidBody.AddRelativeForce(thrust);
                _rigidBody.velocity = new Vector2(Mathf.Clamp(_rigidBody.velocity.x, -_thrustMax, _thrustMax), Mathf.Clamp(_rigidBody.velocity.y, -_thrustMax, _thrustMax));
                if (!GameAudio.I.IsPlaying(GameSounds.Thruster))
                {
                    GameAudio.I.PlayFX(GameSounds.Thruster);
                }
            }
        }

        /// <summary>
        /// Removes jet blast and propulsion sound fx 
        /// </summary>
        public void DisableThrustEffects()
        {
            if (GameAudio.I.IsPlaying(GameSounds.Thruster))
            {
                GameAudio.I.StopFX(GameSounds.Thruster, true, THRUSTER_FADE);
            }

            if (_engineParticleSystem.isPlaying)
            {
                _engineParticleSystem.Stop();
            }
        }

        /// <summary>
        /// Rotate player by given degress
        /// </summary>
        private void ApplyRotation(float rotationThisFrame)
        {
            Gameplay.I.InputManager.DisplayTurnButtonUsage(true, rotationThisFrame < 0);
            float thrustValue = rotationThisFrame * Time.deltaTime * _rotationThrust;

            // Avoid automatic rotating when hitting colliders
            _rigidBody.freezeRotation = true;

            // Apply rotation
            transform.Rotate(Vector3.back * thrustValue);

            // Resume physics control of rotation
            _rigidBody.freezeRotation = false;
        }

        /// <summary>
        /// Sets player position up ready to start whatever cave level has been loaded
        /// </summary>
        public void ResetPlayerPositionAndRotation()
        {
            UnlockMovement();
            transform.position = GameData.CurrentCaveManager.GetStartPosition() + Vector3.up * _collider.bounds.extents.y;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        /// <summary>
        /// Restores all of player's state to default values
        /// </summary>
        public void RestorePlayerState()
        {
            RemoveAllDamage();
            _thrustingHeld = false;
            _explosion.SetActive(false);
            _animator.SetBool(AnimationTags.READY_TO_EXLODE, false);
            _animator.SetBool(AnimationTags.READY_TO_DIE, false);
            _pausedVelocity = Vector3.zero;
            _rotationalValue = 0;
            _engineParticleSystem.Stop();
            _pausedVelocity = Vector3.zero;

        }

        /// <summary>
        /// Activates/deactivates player's colliders to avoid collision errors whie locked
        /// </summary>
        public void ActivateColliders(bool colliderEnabled)
        {
            _collider.enabled = colliderEnabled;
        }

        /// <summary>
        /// Lock all player movements, to be used when pausing gameplay
        /// </summary>
        public void LockMovement()
        {
            _pausedVelocity = _rigidBody.velocity;
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            _rigidBody.gravityScale = 0;
            ActivateColliders(false);
        }

        /// <summary>
        /// Unlock all player movements, to be used when resuming gameplay
        /// </summary>
        public void UnlockMovement()
        {
            _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidBody.gravityScale = _gravityScale;
            _rigidBody.velocity = _pausedVelocity;
            ActivateColliders(true);
        }

        /// <summary>
        /// Reset damage guage, and updates visible apperance of craft/shields
        /// </summary>
        public void RemoveAllDamage()
        {
            _shieldDamage = 0;
            _playerState.sprite = _damageSprites[_shieldDamage];
            _damageState.sprite = ShieldSprites[_shieldDamage];
        }

        /// <summary>
        /// Checks if latest hit was fatal, if not triggers glowing shield affect
        /// </summary>
        private void InflictDamage()
        {
            if (_shieldDamage == _maxShieldDamage)
            {
                Gameplay.I.PauseGame();
                Explode();
            }
            else if (!GameData.GodMode)
            {
                TriggerDamageEffects();
            }
        }

        /// <summary>
        /// Stops any previous shiled animations currently player, and starts fresh new animation
        /// </summary>
        private void TriggerDamageEffects()
        {
            _damageState.gameObject.SetActive(true);
            //GameManager.Instance.Audio.PlayFX(BoosterSound.BounceOfWall);
            if (_hitFading != null)
            {
                StopCoroutine(_hitFading);
                _hitFading = null;
            }
            _hitFading = StartCoroutine(FadeHitIndicator());
            GameAudio.I.PlayFX(GameSounds.BounceOfWall);
        }

        /// <summary>
        /// Briefly appears after collision but fades out, colour determined by latest damage
        /// </summary>
        public IEnumerator FadeHitIndicator()
        {
            _damageState.color = ShieldColors[_shieldDamage];
            Color shieldColor = _damageState.color;
            float t = 0;

            // Update damage sprite
            _playerState.sprite = _damageSprites[_shieldDamage];

            // Update shield/dmg sprites
            _shieldDamage++;
            _damageState.sprite = ShieldSprites[_shieldDamage - 1];

            while (t <= 1f)
            {
                Color newColor = new Color(shieldColor.r, shieldColor.g, shieldColor.b, Mathf.Lerp(1f, 0, t));
                _damageState.color = newColor;
                t += Time.deltaTime;
                yield return null;
            }

            Color endColor = new Color(shieldColor.r, shieldColor.g, shieldColor.b, 0);
            _damageState.color = endColor;

            _hitFading = null; // Needed?
            _damageState.gameObject.SetActive(false);
        }

        /// <summary>
        /// Sets up player object and then runs explosion animation/sfx, then dies
        /// </summary>
        private void Explode()
        {

            // Updates player sprite
            _playerState.sprite = _damageSprites[_shieldDamage];

            // Deactivate damage sprite
            _damageState.gameObject.SetActive(false);
            _engineParticleSystem.gameObject.SetActive(false);

            // Activates explosion object and trigger animation
            _explosion.SetActive(true);
            _animator.SetBool(AnimationTags.READY_TO_EXLODE, true);

            // Play audio
            GameAudio.I.PlayFX(GameSounds.Explode);
        }

        /// <summary>
        /// Visbily removes player from view
        /// </summary>
        public void Die()
        {
            // Dectivates explosion object and end animation
            _animator.SetBool(AnimationTags.READY_TO_EXLODE, false);
            _explosion.SetActive(false);
            Activate(false);
            Gameplay.I.LevelFailed(FailureReason.Crashed);
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(GameTags.GetTagByEnum(TagNames.CrashableObject)))
            {
                if (!GameData.GodMode)
                {
                    InflictDamage();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(GameTags.GetTagByEnum(TagNames.EndZone)))
            {
                _timeOnLandingPad = 0;
            }
            else if (collider.gameObject.CompareTag(GameTags.GetTagByEnum(TagNames.BonusBox)))
            {
                collider.gameObject.GetComponent<BonusBox>().Collected();
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(GameTags.GetTagByEnum(TagNames.EndZone)))
            {
                _timeOnLandingPad = 0;
            }
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag(GameTags.GetTagByEnum(TagNames.EndZone)))
            {
                _timeOnLandingPad += Time.deltaTime;
                if (_timeOnLandingPad > LANDING_HOLD_TIME)
                {
                    StartCoroutine(Gameplay.I.HeldSuccessfullyInEndZone());
                }
            }
        }

        private void OnEnable()
        {
            Gameplay.I.InputManager.OnDPadMoveEvent += DPadActivated;
            Gameplay.I.InputManager.OnThrustPressedEvent += ThrustPressed;
            Gameplay.I.OnPause += LockMovement;
            Gameplay.I.OnUnpause += UnlockMovement;
        }

        private void OnDisable()
        {
            if(Gameplay.HasInstance())
            {
                Gameplay.I.InputManager.OnDPadMoveEvent -= DPadActivated;
                Gameplay.I.InputManager.OnThrustPressedEvent -= ThrustPressed;
                Gameplay.I.OnPause -= LockMovement;
                Gameplay.I.OnUnpause -= UnlockMovement;
            }
        }
    }
}