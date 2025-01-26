using System.Collections;
using GameJammers.GGJ2025.FloppyDisks;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    public abstract class ExplodableBase : MonoBehaviour, IExplodable, ITacticalView {
        ExplodableCollection _collection;

        [Tooltip("Should this immediately explode when the red button is pressed?")]
        [SerializeField]
        bool _autoExplode;

        [Tooltip("Is this explodable an objective that must be destroyed in order to beat the level?")]
        [SerializeField]
        bool _isObjective;

        [Tooltip("You can use any explosion shape you want. The trigger area just needs to have a TriggerExplodableTracker on it.")]
        [SerializeField]
        TriggerExplodableTracker _explosionTrigger;



        public bool AutoExplode => _autoExplode;
        public bool IsObjective => _isObjective;
        public bool IsPrimed { get; private set; }

        void Start () {
            _collection = GameController.Instance.Explodables;
            _collection.Add(this);
            ToggleView(GameController.Instance.IsTacticalViewEnabled);
            GameController.Instance.TacticalViewToggled += ToggleView;
        }

        public void Explode () {
            StartCoroutine(ExplosionLoop());
        }

        IEnumerator ExplosionLoop () {
            // Prevent recursive detonations by setting the primed flag
            IsPrimed = true;

            // Track this globally so we can wait for all explosions to resolve
            _collection.AddExploding(this);

            PlayAnimation();

            while (!GetIsAnimationComplete()) {
                yield return null;
            }

            // Find all explodables inside the explosion area
            foreach (var target in _explosionTrigger.TrackedObjects) {
                if (!target.IsPrimed) {
                    target.Explode();
                }
            }

            // Inform the game state that this explosion has resolved
            _collection.RemoveExploding(this);

            ExplosionComplete();
        }

        void PlayAnimation() {
            OnPlayAnimation();
        }

        protected virtual void OnPlayAnimation() {}

        protected virtual bool GetIsAnimationComplete() {
            return true;
        }

        void ExplosionComplete() {
            OnExplosionComplete();
        }

        protected virtual void OnExplosionComplete() {}

        protected virtual void OnDestroy () {
            _collection.Remove(this);
            GameController.Instance.TacticalViewToggled -= ToggleView;
        }

        public abstract void ToggleView (bool toggle);
    }
}
