using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class RamCounter : MonoBehaviour {
        GameController _game;
        Color _successColor;

        [SerializeField]
        TextMeshProUGUI _total;

        [SerializeField]
        Image _icon;

        [Tooltip("The success color is auto pulled from the text color")]
        [SerializeField]
        Color _errorColor = Color.red;

        void Start () {
            _game = GameController.Instance;
            _game.Ram.EventRamChanged.AddListener(OnRamChanged);

            _successColor = _total.color;

            UpdateDisplay(_game.Ram.Current, _game.Ram.Max);
        }

        void OnDestroy () {
            _game.Ram?.EventRamChanged.RemoveListener(OnRamChanged);
        }

        void OnRamChanged (int current, int max) {
            UpdateDisplay(current, max);
        }

        void UpdateDisplay (int current, int max) {
            _total.text = $"{current}/{max}";

            var color = _game.Ram.IsValid ? _successColor : _errorColor;
            _total.color = color;
            _icon.color = color;
        }
    }
}
