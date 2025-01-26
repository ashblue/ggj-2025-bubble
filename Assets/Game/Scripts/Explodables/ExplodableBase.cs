using GameJammers.GGJ2025.FloppyDisks;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    public abstract class ExplodableBase : MonoBehaviour, IExplodable {
        ExplodableCollection _collection;

        [Tooltip("Should this immediately explode when the red button is pressed?")]
        [SerializeField]
        bool _autoExplode;

        [Tooltip("Is this explodable an objective that must be destroyed in order to beat the level?")]
        [SerializeField]
        bool _isObjective;

        /* Removing this for now. The base poppable can be extended to add colliders or use physics calls.
         The base poppable will use a sphere cast
        [Tooltip("You can use any explosion shape you want. The trigger area just needs to have a TriggerExplodableTracker on it.")]
        [SerializeField]
        TriggerExplodableTracker _explosionTrigger;*/

        public bool AutoExplode => _autoExplode;
        public bool IsObjective => _isObjective;
        public bool IsPrimed { get; private set; } // now only used for scoring

        protected virtual void Start () {
            _collection = GameController.Instance.Explodables;
            _collection.Add(this);
        }

        public void Explode () {
            //StartCoroutine(ExplosionLoop());
            PopManager.Instance.AddPopToQueue(GetComponent<Poppable>()); // always extended by poppable. Could rework, no time
        }

        public void Prime () {
            IsPrimed = true;
            // Track this globally so we can wait for all explosions to resolve
            _collection.AddExploding(this);
            PlayAnimation(); // may hide this for now?

        }

        /*IEnumerator ExplosionLoop () {
            // this functionality is moving over to PopManager
            // Prevent recursive detonations by setting the primed flag
            IsPrimed = true;

            // Track this globally so we can wait for all explosions to resolve
            _collection.AddExploding(this);

            PlayAnimation();
            // timing is handled by PopManager and poppables now
            //while (!GetIsAnimationComplete()) {
            //    yield return null;
            //}

            // there is a slight difference in the implication of IsPrimed and Poppable.CanPop
            // Can Pop will handle this and IsPrimed can be used for scoring
            // Find all explodables inside the explosion area
            //foreach (var target in _explosionTrigger.TrackedObjects) {
            //    if (!target.IsPrimed) {
            //        target.Explode();
            //    }
            //}

            // Inform the game state that this explosion has resolved
            //_collection.RemoveExploding(this);

            //ExplosionComplete();
        }*/

        void PlayAnimation() {
            OnPlayAnimation();
        }

        protected virtual void OnPlayAnimation() {}

        protected virtual bool GetIsAnimationComplete() {
            return true;
        }

        protected void ExplosionComplete() {
            OnExplosionComplete();
        }

        protected virtual void OnExplosionComplete() {}

        protected virtual void OnDestroy () {
            _collection.Remove(this);
        }
    }
}
