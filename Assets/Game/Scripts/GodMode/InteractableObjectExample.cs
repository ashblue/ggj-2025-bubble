using UnityEngine;

namespace GameJammers.GGJ2025.GodMode {
    public class InteractableObjectExample : InteractableObjectBase {
        bool _debug;

        protected override void OnPipInteract () {
            if (!_debug) return;
            Debug.Log("Interacted with " + name);
        }

        protected override void OnPipHover () {
            if (!_debug) return;
            Debug.Log("Hovered over " + name);
        }
    }
}
