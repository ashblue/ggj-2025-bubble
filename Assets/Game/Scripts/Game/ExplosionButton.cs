using ChrisNolet.QuickOutline;
using UnityEngine;
using UnityEngine.Events;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class ExplosionButton : MonoBehaviour {
        GameController _game;
        private Outline _buttonOutline;

        void Start () {
            _game = GameController.Instance;
            TryGetComponent(out _buttonoutline);
            if(_buttonoutline != null) {
                _buttonoutline.enabled = false;
            }
        }

        void OnMouseEnter () {
            if (_buttonoutline != null) _buttonoutline.enabled = true;
        }

        void OnMouseExit () {
            if (_buttonoutline != null) _buttonoutline.enabled = false;
        }

        void OnMouseDown () {
            _buttonoutline.OutlineColor = Color.magenta;

            if (!_game) {
                Debug.LogWarning("Explosions cannot be tested without a GameController instance. You probably need to additively load the Main scene to globally load it.");
                return;
            }

            if (_game.State != GameState.Placement) return;
            _game.Exploder.Begin();
        }
    }
}
