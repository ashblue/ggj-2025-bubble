using UnityEditor;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    [CustomEditor(typeof(ExplodableExample))]
    public class ExplodableEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            // Add a button to the inspector when the game is playing
            if (Application.isPlaying) {
                var explodable = (ExplodableExample)target;

                if (GUILayout.Button("Explode")) {
                    explodable.Explode();
                }
            }
        }
    }
}
