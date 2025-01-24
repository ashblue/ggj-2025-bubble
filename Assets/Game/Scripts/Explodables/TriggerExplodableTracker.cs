using System.Collections.Generic;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    public class TriggerExplodableTracker : MonoBehaviour {
        readonly List<Explodable> _trackedObjects = new();

        public IReadOnlyList<Explodable> TrackedObjects => _trackedObjects;

        void OnTriggerEnter (Collider other) {
            var explodable = other.GetComponent<Explodable>();
            if (explodable) {
                _trackedObjects.Add(explodable);
            }
        }

        void OnTriggerExit (Collider other) {
            var explodable = other.GetComponent<Explodable>();
            if (explodable) {
                _trackedObjects.Remove(explodable);
            }
        }
    }
}
