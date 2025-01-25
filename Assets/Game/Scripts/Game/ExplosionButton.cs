using ChrisNolet.QuickOutline;
using UnityEngine;
using UnityEngine.Events;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class ExplosionButton : MonoBehaviour {
        GameController _game;
        private Outline _buttonOutline;

        void Start () {
            _game = GameController.Instance;
            TryGetComponent(out _buttonOutline);
            if(_buttonOutline != null) {
                _buttonOutline.enabled = false;
            }
        }

        void OnMouseEnter () {
            if (_buttonOutline != null) _buttonOutline.enabled = true;
        }

        void OnMouseExit () {
            if (_buttonOutline != null) _buttonOutline.enabled = false;
        }

        void OnMouseDown () {
            if (_buttonOutline != null) _buttonOutline.OutlineColor = Color.magenta;

            if (!_game) {
                Debug.LogWarning("Explosions cannot be tested without a GameController instance. You probably need to additively load the Main scene to globally load it.");
                return;
            }

            if (_game.State != GameState.Placement) return;
            _game.Exploder.Begin();
        }
    }
}
