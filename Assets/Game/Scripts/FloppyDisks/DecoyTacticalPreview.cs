using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class DecoyTacticalPreview : MonoBehaviour, ITacticalView {

        [SerializeField] GameObject _explosionRadius;

        public void ToggleView (bool toggle) {
            if (_explosionRadius == null) { return; }
            _explosionRadius.SetActive(toggle);
        }
    }
}
