using UnityEngine;

namespace GameJammers.GGJ2025.Emote {
    public static class EyePoseRegistryLoader {
        private static EyePoseRegistry eyePoseRegistry;

        // Load the registry dynamically
        public static EyePoseRegistry LoadRegistry() {
            if (eyePoseRegistry == null) {
                eyePoseRegistry = Resources.Load<EyePoseRegistry>("EyePoses/EyePoseRegistry");

                if (eyePoseRegistry == null) {
                    Debug.LogError("EyePoseRegistry could not be loaded from Resources.");
                } else {
                    Debug.Log("EyePoseRegistry loaded successfully.");
                }
            }
            return eyePoseRegistry;
        }
    }

}
