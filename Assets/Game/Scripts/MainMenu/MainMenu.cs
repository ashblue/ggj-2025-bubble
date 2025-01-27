using GameJammers.GGJ2025.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace GameJammers.GGJ2025.UI {
    public class MainMenu : MonoBehaviour {

        public int LevelsCleared { get; set; } = 0;

        public string gameScene;

        [SerializeField] Button _resumeButton;
        [SerializeField] GameObject _creditsMenu;
        [SerializeField] GameObject _startMenu;


        void Start () {
            _resumeButton.gameObject.SetActive(LevelsCleared > 0);
            _startMenu.SetActive(true);
            _creditsMenu.SetActive(false);

            MusicManager.Instance.UpdateTrack(1);
        }

        public void LoadGameScene (int level) {
            var loading = SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Single);
            void OnLoaded (AsyncOperation op) {
                // Do some logic here to pass the level number to the scene
                // so the appropriate level is loaded.
                loading.completed -= OnLoaded;
            }
            loading.completed += OnLoaded;
        }

        public void OnStartButtonPressed () {
            MusicManager.Instance.UpdateTrack(1);
            LoadGameScene(0);

        }

        public void MenuSound () {
            Debug.Log ("MenuSound");
            FMODUnity.RuntimeManager.PlayOneShot("Events:/MenuNav");
        }

        public void OnResumeButtonPressed () {
            MenuSound();
            LoadGameScene(LevelsCleared);
        }

        public void OnCreditsButtonPressed () {
            MenuSound();
            _startMenu.SetActive(false);
            _creditsMenu.SetActive(true);
        }

        public void OnReturnFromCreditsPressed () {
            MenuSound();
            _startMenu.SetActive(true);
            _creditsMenu.SetActive(false);
        }

        public void OnQuitButtonPressed () => Application.Quit();
    }
}

