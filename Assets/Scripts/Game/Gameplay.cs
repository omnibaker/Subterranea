using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Subterranea
{
    /// <summary>
    /// Manages all the gameplay scene flow including player, cave/world, UI, and input controls
    /// </summary>
    public class Gameplay : Singleton<Gameplay>
    {
        [SerializeField] private GameObject _playerPF = default; // player craft prefab
        [SerializeField] private PlayUI _playUI = default; // in-game UI controller
        [SerializeField] private BlackFader _fader = default; // 
        [SerializeField] private GamesInputManager _inputManager = default; // InputAction for mobile 

        public GamesInputManager InputManager { get { return _inputManager;} }

        // Pause references used when controlling actor movements when play needs to stop temporarily
        public UnityAction OnPause { set; get; }
        public UnityAction OnUnpause { set; get; }
        public bool IsPaused { get; set; }

#if UNITY_STANDALONE
        private const string KEYBOARD_COMMANDS = "\nSPACE = THRUSTER\nA = ROTATE LEFT\nD = ROTATE RIGHT\n";
#endif

        private Player _player;
        private PlayerChaser _playerChaser = default;
        private GameObject _currentLevelPrefab;
        private bool _timerActive = false;

        public void Awake()
        {
            CreateInstance(this, gameObject);
        }

        public void Start()
        {
#if UNITY_STANDALONE
            RemoveScreenControllersForDesktop();
#endif
            StartNewGame(true);
        }

        public void Update()
        {
            UpdateCountdownTime();
        }

        public void OnEnable()
        {
#if UNITY_IOS
        Application.targetFrameRate = 60;
#endif
            OnPause += () => IsPaused = true;
            OnPause += () => _inputManager.DisplayTurnButtonUsage(false);
            OnPause += () => _inputManager.DisplayThrustButtonUsage(false);
            OnUnpause += () => IsPaused = false;
        }

        public void OnDisable()
        {
#if UNITY_IOS
        Application.targetFrameRate = -1;
#endif
            OnPause -= () => IsPaused = true;
            OnUnpause -= () => IsPaused = false;
        }


        /// <summary>
        /// Runs processes that occur when a brand new game is started
        /// </summary>
        public void StartNewGame(bool arrivedFromMenu = false)
        {
            // Make sure game start blacked out
            _fader.StraightToBlack();

            // Run the game reset
            PauseGame();

            // Reset all the settings values
            ResetValuesForNewGame();

            // Close all question boxes
            _playUI.RemoveAllPopUps();

            // Initial a new level
            StartCoroutine(StartNewCave(true, arrivedFromMenu));
        }

        /// <summary>
        /// Runs processes that occur when a new level/wave is started
        /// </summary>
        public IEnumerator StartNewCave(bool newLevel = false, bool newGame = false)
        {
            // If not a respawn, create cave level and update current Cave data
            if (newLevel)
            {
                if (GameData.CurrentCaveManager != null)
                {
                    GameData.CurrentCaveManager.RemoveCave();
                    GameData.CurrentCaveManager = null;
                }
                GameData.CurrentCaveManager = CreateLevelFromPrefab();
            }

            // Restore any bonuses already aquired
            ReactivateAllBonuses();

            //  Reset time
            GameData.TimeRemaining = (int)GameData.CurrentCaveManager.Cave.TimeLimit;

            // Reset/pause timer and update level field
            _timerActive = false;
            _playUI.UpdateRemainingTime($"{GameUtils.GetTimeInFormattedString(GameData.TimeRemaining)}");
            _playUI.UpdateLevelDisplay();

            // Display keyboard controls for desktop users
            if (newGame)
            {
                yield return StartCoroutine(DisplayKeyboardControls());
            }

            // Reset player start state
            PreparePlayer();

            yield return new WaitForSeconds(1f);

            _playUI.UpdateMessage("GET READY");

            yield return new WaitForSeconds(1f);

            // Start to fade in
            _fader.StartFadeToVisible();

            yield return new WaitForSeconds(2f);

            // Unpause and start timer
            _playUI.UpdateMessage("GO!");
            _timerActive = true;
            UnpauseGame();

            // Remove display
            yield return new WaitForSeconds(1.5f);
            _playUI.ClearMessage();
        }

        /// <summary>
        /// Process that runs when player lands on current cave's end platform
        /// </summary>
        public IEnumerator HeldSuccessfullyInEndZone()
        {
            // Trigger success sound effect
            GameAudio.I.PlayFX(GameSounds.LandedInEndZone);

            // Stop timer and player
            _timerActive = false;
            PauseGame();

            // Display Message
            _playUI.UpdateMessage($"CAVE {GameData.CurrentLevel}-{GameData.CurrentCave} \nCOMPLETE!");

            // Wait 1 second, remove message, 
            yield return new WaitForSeconds(1f);
            _fader.StartFadeToBlack();

            // Update score and calcualte the bonus points
            UpdateScoreForFinishingLevel();
            _playUI.ClearMessage();
            if (GameData.TimeRemaining >= 1f)
            {
                yield return StartCoroutine(_playUI.CreateBonusPointsPopUp());
            }

            yield return new WaitForSeconds(1f);

            // Update level/cave details
            bool endGame = IncrementToNextCaveLevel(GameData.CurrentLevel, GameData.CurrentCave);

            // Update any important metrics
            GameData.CheckData();

            // Start a new level
            _player.Activate(false);

            if(!endGame)
            {
                StartCoroutine(StartNewCave(true));
            }
        }

        /// <summary>
        /// Runs EOL processes that occur when Enemy hits ground level
        /// </summary>
        public void LevelFailed(FailureReason reason)
        {
            // Stop timer and player
            _timerActive = false;
            PauseGame();

            GameData.CheckData();

            if (GameData.CurrentLives > 0)
            {
                StartCoroutine(FailedButPlayAgain(reason));
            }
            else // Game Over
            {
                StartCoroutine(GameOver());
            }
        }

        /// <summary>
        /// Runs process of starting a new life after dying
        /// </summary>
        public IEnumerator FailedButPlayAgain(FailureReason reason)
        {
            // Remvoe life icon from display
            _playUI.UpdateLives(--GameData.CurrentLives);

            // Display "Dead" message
            _playUI.UpdateMessage(FailedMessage(reason));

            yield return new WaitForSeconds(1f);

            // Fade out
            _fader.StartFadeToBlack();

            yield return new WaitForSeconds(2f);

            // Restart level
            _player.Activate(false);
            StartCoroutine(StartNewCave());
        }

        /// <summary>
        /// Process that runs when last cave has been completed
        /// </summary>
        public void GameIsCompleted()
        {
            _playUI.CreateOptionsPopUp(OptionPanelType.GameCompleted);
        }

        /// <summary>
        /// Runs process when dying with no lives remaining
        /// </summary>
        public IEnumerator GameOver()
        {
            // Fade out and display message
            _fader.StartFadeToBlack();
            _playUI.UpdateMessage($"GAME OVER!!!");

            yield return new WaitForSeconds(2f);
            _player.Activate(false);

            _playUI.CreateOptionsPopUp(OptionPanelType.GameOver);
        }

        /// <summary>
        /// If desktop user, displays keyboard controls briefly
        /// </summary>
        public IEnumerator DisplayKeyboardControls()
        {
#if UNITY_STANDALONE
            _playUI.UpdateMessage(KEYBOARD_COMMANDS, 3f);
            yield return new WaitForSeconds(3f);
#endif
            yield return null;
        }

        /// <summary>
        /// Resets data and UI elements for starting a new game
        /// </summary>
        public void ResetValuesForNewGame()
        {
            // Reset local data
            GameData.ResetLevelsAndScores(false);

            // Reset HUD values
            _playUI.ResetUI();
        }

        /// <summary>
        /// Instantiates level prefab into scene
        /// </summary>
        public CaveManager CreateLevelFromPrefab()
        {
            // Destroy any previous cave level
            if (_currentLevelPrefab != null)
            {
                Destroy(_currentLevelPrefab);
            }

            // Confirm level key exists
            if (GameData.Levels.TryGetValue(GameData.CurrentLevel, out List<GameObject> caveLevels))
            {
                // Confirm level has enough caves
                if (caveLevels.Count >= GameData.CurrentCave)
                {
                    // Create prefab and return it
                    _currentLevelPrefab = Instantiate(caveLevels[GameData.CurrentCave - 1]);
                    return _currentLevelPrefab.GetComponent<CaveManager>();
                }
                else
                {
                    Debug.LogError($"Level '{GameData.CurrentLevel}' does not have Cave #{ GameData.CurrentCave}");
                }
            }
            else
            {
                Debug.LogError($"Level '{GameData.CurrentLevel}' is not a valid key");
            }

            return null;
        }

        /// <summary>
        /// Resets player on to scene
        /// </summary>
        public void PreparePlayer()
        {
            if (_player == null)
            {
                _player = CreatePlayerAndAlertCamera();
            }

            // Set up player for next cave
            _player.RestorePlayerState();
            _player.ResetPlayerPositionAndRotation();
            _player.Activate(true);
        }

        /// <summary>
        /// Create new player and attaches camera chaser
        /// </summary>
        private Player CreatePlayerAndAlertCamera()
        {
            GameObject player = Instantiate(_playerPF);
            if (_playerChaser == null)
            {
                _playerChaser = FindObjectOfType<PlayerChaser>();
            }
            _playerChaser.AssignChasedObject(player.transform);
            return player.GetComponent<Player>();
        }

        /// <summary>
        /// Used when no Main Menu screen. Assumed for test usage only.
        /// </summary>
        private void AutomaticPlay()
        {
            Debug.Log("TODO: GameManager NOT detected | TODO: ProxyDataCreator");
        }

        /// <summary>
        /// Slowly but incrementally counts down the time remaining and displays it by the second by the second
        /// </summary>
        public void UpdateCountdownTime()
        {
            if (!GameData.GodMode && !IsPaused)
            {
                if (_timerActive)
                {
                    GameData.TimeRemaining -= Time.deltaTime;
                    if (GameData.TimeRemaining <= 0)
                    {
                        _timerActive = false;
                        LevelFailed(FailureReason.OutOfTime);
                    }
                    else
                    {
                        _playUI.UpdateRemainingTime($"{GameUtils.GetTimeInFormattedString(GameData.TimeRemaining)}");
                    }
                }
            }
        }

        /// <summary>
        /// Cleans data and returns to StartMenu scene
        /// </summary>
        public void QuitPlay()
        {
            // Reset all game data
            GameData.ResetLevelsAndScores(true);
            _playUI.ResetUI();
            _playUI.RemoveAllPopUps();

            // Go back to menu
            SceneLoader.I.GoToScene(SceneName.STARTMENU);
        }

        /// <summary>
        /// Cleans data and set current level to last selected level (or 1 by default)
        /// </summary>
        public void RefreshForNewGame()
        {
            // Reset all game values
            GameData.ResetLevelsAndScores(false);
            _playUI.ResetUI();
            _playUI.RemoveAllPopUps();
            GameData.SetCurrentLevelAndCave(GameData.SelectedLevel);
        }

        /// <summary>
        /// Triggers paused behaviour
        /// </summary>
        public void PauseGame()
        {
            if (!IsPaused)
            {
                OnPause?.Invoke();
            }
        }

        /// <summary>
        /// Triggers unpaused behaviour
        /// </summary>
        public void UnpauseGame()
        {
            if (IsPaused)
            {
                OnUnpause?.Invoke();
            }
        }

        /// <summary>
        /// Returns reason for failure to be displayed on screen
        /// </summary>
        private string FailedMessage(FailureReason failure)
        {
            switch (failure)
            {
                case FailureReason.Crashed: return "CRASHED!";
                case FailureReason.OutOfTime: return "OUT OF TIME!";
            }

            return "LEVEL FAILED!";
        }

        /// <summary>
        /// Simple points added for completing a cave
        /// </summary>
        public void UpdateScoreForFinishingLevel()
        {
            GameData.CurrentScore += LevelSettings.POINT_FOR_LEVELUP;
            GameData.CheckHighestScore();
            _playUI.UpdateScore(GameData.CurrentScore);

        }

        /// <summary>
        /// Makes any collected bonuses during previous round reappear
        /// </summary>
        private void ReactivateAllBonuses()
        {
            BonusBox[] bonusBoxes = FindObjectsOfType<BonusBox>();
            if (bonusBoxes.Length > 0)
            {
                foreach (BonusBox bb in bonusBoxes)
                {
                    bb.ReactivateBonus();
                }
            }
        }

        /// <summary>
        /// Triggers rewards received when collecting health object
        /// </summary>
        public void BonusFullShields()
        {
            _player.RemoveAllDamage();
        }

        /// <summary>
        /// Triggers rewards received when collecting life object
        /// </summary>
        public void BonusOneUp()
        {
            _playUI.UpdateLives(++GameData.CurrentLives);
        }        
 
        /// <summary>
        /// When cave is complete, jumps to new level is all current level's cave completed
        /// </summary>
        public bool IncrementToNextCaveLevel(int currentLevel, int currentCave)
        {
            // Check if any caves left in level
            if (currentCave < GameData.Levels[currentLevel].Count)
            {
                GameData.CurrentCave++;
                return false;
            }
            else
            {
                int newLevel = currentLevel;

                // Find next level that has caves and update
                while (true)
                {
                    newLevel++;

                    // If no more levels - game completed
                    if (newLevel > GameData.Levels.Count)
                    {
                        Debug.LogError("NO MORE LEVELS LEFT - TELL USER THEY HAVE BEATEN GAME");
                        GameIsCompleted();
                        return true;
                    }
                    // Update level and make CurrentCave first in level
                    else if (GameData.Levels[newLevel].Count > 0)
                    {
                        GameData.CurrentLevel = newLevel;

                        // Add to PlayerPref is new level unlocked
                        if (GameData.CurrentLevel > PlayerPrefs.GetInt(PrefRef.HIGHEST_LEVEL, 1))
                        {
                            PlayerPrefs.SetInt(PrefRef.HIGHEST_LEVEL, GameData.CurrentLevel);
                        }

                        GameData.CurrentCave = 1;
                        return false;
                    }
                }
            }
        }


        /// <summary>
        /// Desktop users only need keyboard, on-screen controller buttons removed
        /// </summary>
        private void RemoveScreenControllersForDesktop()
        {
            InputManager.RemoveMobileInputs();
        }
    }
}