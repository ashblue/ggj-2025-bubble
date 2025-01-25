using UnityEngine;
using System.Collections.Generic;

namespace GameJammers.GGJ2025.Emote {
    [CreateAssetMenu(fileName = "EyePoseRegistry", menuName = "GGJ/Emotes/EyePoseRegistry", order = 2)]
    public class EyePoseRegistry : ScriptableObject {
        [SerializeField] public List<EyePose> eyePoses = new List<EyePose>();

        private Dictionary<string, EyePose> poseLookup;

        private void OnEnable() {
            // Initialize the lookup dictionary for quick access
            poseLookup = new Dictionary<string, EyePose>();
            foreach (var pose in eyePoses) {
                if (pose != null && !string.IsNullOrEmpty(pose.Name)) {
                    poseLookup[pose.Name] = pose;
                }
            }
        }

        public EyePose GetPose(string name) {
            if (poseLookup.ContainsKey(name)) {
                return poseLookup[name];
            }
            else {
                Debug.LogWarning("No eye pose registered for " + name);
                return null;
            }
        }
    }
}
