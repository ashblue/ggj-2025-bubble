using ChrisNolet.QuickOutline;
using UnityEngine;

#nullable enable

namespace GameJammers.GGJ2025.GodMode {
    public class OutlineOnClickExample : InteractableObjectBase {
        private Outline _outline = default!;

        private void Awake() {
            if (TryGetComponent(out Outline res)) {
                _outline = res;
                _outline.enabled = false;
            }
            else {
                Debug.LogWarning("Can't find Outline Component. Disabling");
                gameObject.SetActive(false);
            }
        }

        protected override void OnPipInteract() {
            _outline.enabled = !_outline.enabled;
        }
    }
}

