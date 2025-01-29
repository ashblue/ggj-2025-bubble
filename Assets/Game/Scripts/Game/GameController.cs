using System;
using System.Collections;
using System.Collections.Generic;
using GameJammers.GGJ2025.Audio;
using GameJammers.GGJ2025.Explodables;
using GameJammers.GGJ2025.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class GameController : MonoBehaviour, ICoroutineRunner {
        public static GameController Instance { get; private set; }

        int _level;
        string _currentLevelPath;
        bool _endGame;

        [Tooltip("The room scene that should be loaded into the game")]
        [SerializeField]
        string _roomScenePath;

        [Tooltip("A list of level scene paths. They will be loaded in order each time the current level is beaten. Note the scene path must be included for additive level debugging to prevent double loads")]
        [SerializeField]
        List<string> _levelScenePaths;

        [Header("Dependencies")]

        [SerializeField]
        ExplodeController _exploder;

        [Tooltip("Camera for initial loading until a real camera exists from the additive scene loads")]
        [SerializeField]
        Camera _tmpCamera;

        [Tooltip("Loading screen when levels are being swapped or initially loaded")]
        [SerializeField]
        Canvas _loadingScreen;

        [Tooltip("Screen shown when the game is complete")]
        [SerializeField]
        Canvas _gameCompleteScreen;

        [Header("Events")]

        [SerializeField]
        UnityEvent _eventGameComplete;

        [SerializeField]
        UnityEvent _eventGameReady;

        public ExplodeController Exploder => _exploder;
        public UnityEvent EventGameReady => _eventGameReady;
        public UnityEvent EventGameComplete => _eventGameComplete;
        public GameState State { get; private set; }
        public ExplodableCollection Explodables { get; } = new();
        public RamController Ram { get; } = new();

        public event Action<bool> TacticalViewToggled;
        public bool IsTacticalViewEnabled {
            get => _isTacticalViewEnabled;
            private set {
                if (_isTacticalViewEnabled != value) {
                    TacticalViewToggled?.Invoke(value);
                }
                _isTacticalViewEnabled = value;
            }
        }
        bool _isTacticalViewEnabled = false;
        InputAction _toggleTacticalViewAction;


        void Awake () {
            Instance = this;
            _loadingScreen.gameObject.SetActive(true);
            _exploder = new ExplodeController(this, Explodables, this);
            _toggleTacticalViewAction = InputSystem.actions.FindAction("ToggleTacticalView");
            _toggleTacticalViewAction.started += OnTacticalModeToggleOn;
            _toggleTacticalViewAction.canceled += OnTacticalModeToggleOff;
        }

        void OnDestroy () {
            if (Instance == this) Instance = null;
            _toggleTacticalViewAction.started -= OnTacticalModeToggleOn;
            _toggleTacticalViewAction.canceled -= OnTacticalModeToggleOff;
        }

        void Start () {
            // Do not load the game if any scene paths will crash
            if (!VerifyAllScenePaths()) {
                Debug.LogError("Some scenes could not be found. Please be sure they have been added to the build settings and try again.");
                return;
            }

            // @TODO Save and load the current level index on boot via player prefs
            StartCoroutine(LoadGameLoop(_roomScenePath, _levelScenePaths[0]));
        }

        bool VerifyAllScenePaths () {
            var valid = true;

            // Verify room scene path exists
            if (SceneUtility.GetBuildIndexByScenePath(_roomScenePath) == -1) {
                Debug.LogWarning($"Room scene path {_roomScenePath} does not exist. Please fix and run the game again.");
                valid = false;
            }

            // Verify all level scene paths exist
            foreach (var levelScenePath in _levelScenePaths) {
                if (SceneUtility.GetBuildIndexByScenePath(levelScenePath) == -1) {
                    Debug.LogWarning($"Level scene path {levelScenePath} does not exist. Please fix and run the game again.");
                    valid = false;
                }
            }

            // Verify at least one level scene path exists
            if (_levelScenePaths.Count == 0) {
                Debug.LogWarning("No level scene paths were provided. Please fix and run the game again.");
                valid = false;
            }

            return valid;
        }

        IEnumerator LoadGameLoop (string roomScenePath, string levelScenePath) {
            // Double check we aren't already loaded for live additive scene debugging
            AsyncOperation room = null;
            var isRoomLoaded = SceneManager.GetSceneByPath(roomScenePath).isLoaded;

            // Double check we aren't already loaded for live editing a level
            Coroutine level = null;
            var isLevelLoaded = false;
            foreach (var levelScene in _levelScenePaths) {
                if (!SceneManager.GetSceneByPath(levelScene).isLoaded) continue;
                isLevelLoaded = true;
                _currentLevelPath = levelScene;
                break;
            }

            // Additive load the files
            if (!isRoomLoaded) room = SceneManager.LoadSceneAsync(roomScenePath, LoadSceneMode.Additive);
            if (!isLevelLoaded) level = StartCoroutine(LoadLevelLoop(levelScenePath));
            if (_currentLevelPath == null) _currentLevelPath = levelScenePath;

            // Make sure everything is done loading;
            if (!isRoomLoaded) yield return room;
            yield return level;

            HideLoadingScreen();

            _eventGameReady.Invoke();
        }

        IEnumerator LoadLevelLoop (string levelPath) {
            // Unload an existing level if it exists and wait for it to resolve
            if (_currentLevelPath != null) {
                var unloadOperation = SceneManager.UnloadSceneAsync(_currentLevelPath);
                yield return unloadOperation;
            }

            // Load the level
            var level = SceneManager.LoadSceneAsync(levelPath, LoadSceneMode.Additive);
            _currentLevelPath = levelPath;
            MusicManager.Instance.UpdateTrack(_level == 0 ? 2 : 3);

            yield return level;
        }

        public void LoadNextLevel () {
            if (_endGame) return;

            _level++;

            // Check if the game has been beaten
            if (_level >= _levelScenePaths.Count) {
                _eventGameComplete.Invoke();
                _endGame = true;
                _gameCompleteScreen.gameObject.SetActive(true);
                return;
            }

            // Get the next level index string and run LoadLevelLoop
            var nextLevelPath = _levelScenePaths[_level];
            StartCoroutine(LoadNextLevelLoop(nextLevelPath));
        }

        IEnumerator LoadNextLevelLoop (string path) {
            ShowLoadingScreen();
            yield return StartCoroutine(LoadLevelLoop(path));
            HideLoadingScreen();

            // Reset the game state
            SetState(GameState.Placement);
            CursorInteractController.Instance.Reset();
        }

        void ShowLoadingScreen () {
            // @TODO If time permits, make it so additional level swaps show a message on the computer screen instead of a full screen overlay
            // https://trello.com/c/UJIlsgWf/17-when-a-level-loads-display-a-loading-message-on-the-screen-instead-of-activating-a-full-screen-overlay
            _loadingScreen.gameObject.SetActive(true);
        }

        void HideLoadingScreen () {
            _tmpCamera.gameObject.SetActive(false);
            _loadingScreen.gameObject.SetActive(false);
        }

        public void SetState (GameState state) {
            State = state;
        }

        public void RestartLevel () {
            StartCoroutine(LoadNextLevelLoop(_currentLevelPath));
        }

        private void OnTacticalModeToggleOn(InputAction.CallbackContext ctx) => IsTacticalViewEnabled = true;

        private void OnTacticalModeToggleOff (InputAction.CallbackContext ctx) => IsTacticalViewEnabled = false;
    }
}
