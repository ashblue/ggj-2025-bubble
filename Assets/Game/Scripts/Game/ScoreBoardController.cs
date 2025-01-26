using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class ScoreBoardController : MonoBehaviour {
        [SerializeField]
        RectTransform _winScreen;

        [SerializeField]
        RectTransform _failScreen;

        [SerializeField]
        TextMeshProUGUI _failText;

        public static ScoreBoardController Instance { get; private set; }

        void Start () {
            Instance = this;

            _winScreen.gameObject.SetActive(false);
            _failScreen.gameObject.SetActive(false);
        }

        void OnDestroy () {
            if (Instance == this) Instance = null;
        }

        public void Play () {
            var ramMax = GameController.Instance.Ram.Max;
            var ramScore = GameController.Instance.Ram.Current;

            var objectives = GameController.Instance.Explodables.Items.Count(sel => sel.IsObjective);
            var objectivesComplete =
                GameController.Instance.Explodables.Items.Count(sel => sel.IsObjective && sel.IsPoppedSuccess);

            GameController.Instance.SetState(GameState.Scoring);

            if (objectivesComplete == objectives && ramScore <= ramMax) {
                _winScreen.gameObject.SetActive(true);
                StartCoroutine(ClickToNextLevelLoop());
            } else {
                var failText = "";
                var failedObjectives = objectives - objectivesComplete;
                var ramOverflow = ramScore - ramMax;
                if (failedObjectives > 0) failText += $"Failed {failedObjectives} objectives.\n";
                if (ramOverflow > 0) failText += $"Exceeded {ramOverflow} RAM. Reduce floppy disk usage.\n";
                _failText.text = failText;

                _failScreen.gameObject.SetActive(true);
                StartCoroutine(ClickToRestartLoop());
            }
        }

        IEnumerator ClickToNextLevelLoop () {
            while (!Input.GetMouseButtonDown(0)) {
                yield return null;
            }

            GameController.Instance.LoadNextLevel();
        }

        IEnumerator ClickToRestartLoop () {
            while (!Input.GetMouseButtonDown(0)) {
                yield return null;
            }

            GameController.Instance.RestartLevel();
        }
    }
}
