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

        public bool AutoExplode => _autoExplode;
        public bool IsObjective => _isObjective;
        public bool IsPrimed { get; private set; } // now only used for scoring
        public virtual bool IsPoppedSuccess => IsPrimed;

        protected virtual void Start () {
            _collection = GameController.Instance.Explodables;
            _collection.Add(this);
            ToggleView(GameController.Instance.IsTacticalViewEnabled);
            GameController.Instance.TacticalViewToggled += ToggleView;
        }

        public void Explode () {
            //StartCoroutine(ExplosionLoop());
            PopManager.Instance.AddPopToQueue(GetComponent<Poppable>()); // always extended by poppable. Could rework, no time
        }

        public void Prime () {
            if (!IsPrimed) {
                IsPrimed = true;
                // Track this globally so we can wait for all explosions to resolve
                _collection.AddExploding(this);
                PlayAnimation(); // may hide this for now?
            }
        }

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
            if (GameController.Instance != null) {
                GameController.Instance.TacticalViewToggled -= ToggleView;
            }

            GameController.Instance?.Explodables.Remove(this);
        }

        public abstract void ToggleView (bool toggle);
    }
}
