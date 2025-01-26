using UnityEditor;
using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    [CustomEditor(typeof(GameController))]
    public class GameControllerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // Add a button to the inspector when the game is playing
            if (Application.isPlaying) {
                GameController controller = (GameController)target;

                if (GUILayout.Button("Load Next Level")) {
                    controller.LoadNextLevel();
                }
            }
        }
    }
}
