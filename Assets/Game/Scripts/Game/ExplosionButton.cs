using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class ExplosionButton : MonoBehaviour {
        GameController _game;

        void Start () {
            _game = GameController.Instance;
        }

        void OnMouseDown () {
            if (!_game) {
                Debug.LogWarning("Explosions cannot be tested without a GameController instance. You probably need to additively load the Main scene to globally load it.");
                return;
            }

            if (_game.State != GameState.Placement) return;
            _game.Exploder.Begin();
        }
    }
}
