using System.Collections.Generic;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    public class TriggerExplodableTracker : MonoBehaviour {
        readonly List<ExplodableBase> _trackedObjects = new();

        public IReadOnlyList<ExplodableBase> TrackedObjects => _trackedObjects;

        void OnTriggerEnter (Collider other) {
            var explodable = other.gameObject.GetComponent<ExplodableBase>();
            if (explodable) {
                _trackedObjects.Add(explodable);
            }
        }

        void OnTriggerExit (Collider other) {
            var explodable = other.gameObject.GetComponent<ExplodableBase>();
            if (explodable) {
                _trackedObjects.Remove(explodable);
            }
        }
    }
}
