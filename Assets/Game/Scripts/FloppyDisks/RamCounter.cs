using TMPro;
using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class RamCounter : MonoBehaviour {
        GameController _game;

        [SerializeField]
        TextMeshProUGUI _total;

        void Start () {
            _game = GameController.Instance;
            _game.Ram.EventRamChanged.AddListener(OnRamChanged);

            OnRamChanged(_game.Ram.Current, _game.Ram.Max);
        }

        void OnDestroy () {
            _game.Ram?.EventRamChanged.RemoveListener(OnRamChanged);
        }

        void OnRamChanged (int current, int max) {
            _total.text = $"{current}/{max}";
        }
    }
}
