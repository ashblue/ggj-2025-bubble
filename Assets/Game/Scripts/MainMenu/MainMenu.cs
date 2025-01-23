using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace GameJammers.GGJ2025.UI {
    public class MainMenu : MonoBehaviour {

        public int LevelsCleared { get; set; } = 0;

        public string targetScene = "";

        [SerializeField] Button _resumeButton;
        [SerializeField] GameObject _creditsMenu;
        [SerializeField] GameObject _startMenu;


        void Start () {
            _resumeButton.gameObject.SetActive(LevelsCleared > 0);
            _startMenu.SetActive(true);
            _creditsMenu.SetActive(false);
        }

        public void LoadGameScene (int level) {
            var loading = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
            void OnLoaded (AsyncOperation op) {
                // Do some logic here to pass the level number to the scene
                // so the appropriate level is loaded.
                Debug.Log($"Would have loaded level {level}");
                loading.completed -= OnLoaded;
            }
            loading.completed += OnLoaded;
        }

        public void OnStartButtonPressed () => LoadGameScene(0);

        public void OnResumeButtonPressed () => LoadGameScene(LevelsCleared);

        public void OnCreditsButtonPressed () {
            _startMenu.SetActive(false);
            _creditsMenu.SetActive(true);
        }

        public void OnReturnFromCreditsPressed () {
            _startMenu.SetActive(true);
            _creditsMenu.SetActive(false);
        }

        public void OnQuitButtonPressed () => Application.Quit();
    }
}

