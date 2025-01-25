using ChrisNolet.QuickOutline;
using UnityEngine;
using UnityEngine.Events;

namespace GameJammers.GGJ2025.Bootstraps {
    public class ExplosionButton : MonoBehaviour {
        GameController _game;
        private Outline buttonoutline;

        void Start () {
            _game = GameController.Instance;
            TryGetComponent(out buttonoutline);
            if(buttonoutline != null) {
                buttonoutline.enabled = false;
            }
        }

        void OnMouseEnter () {
            if (buttonoutline == null) { return; }
            buttonoutline.enabled = true;
        }

        void OnMouseExit () {
            if (buttonoutline == null) { return; }
            buttonoutline.enabled = false;
        }

        void OnMouseDown () {
            if (buttonoutline != null) {
                buttonoutline.OutlineColor = Color.magenta;
            }

            if (!_game) {
                Debug.LogWarning("Explosions cannot be tested without a GameController instance. You probably need to additively load the Main scene to globally load it.");
                return;
            }

            if (_game.State != GameState.Placement) return;
            _game.Exploder.Begin();
        }
    }
}
