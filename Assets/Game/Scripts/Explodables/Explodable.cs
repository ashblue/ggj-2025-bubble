using System.Collections;
using DG.Tweening;
using GameJammers.GGJ2025.Bootstraps;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    public class Explodable : MonoBehaviour, IExplodable {
        ExplodableCollection _collection;

        [Tooltip("Should this immediately explode when the red button is pressed?")]
        [SerializeField]
        bool _isPrimer;

        [Tooltip("Is this explodable an objective that must be destroyed in order to beat the level?")]
        [SerializeField]
        bool _isObjective;

        [Tooltip("You can use any explosion shape you want. The trigger area just needs to have a TriggerExplodableTracker on it.")]
        [SerializeField]
        TriggerExplodableTracker _explosionTrigger;

        [Header("Animation")]

        [SerializeField]
        float _explosionDelay = 0.5f;

        [SerializeField]
        GameObject _explosionGraphic;

        [SerializeField]
        float _explosionDuration = 0.5f;

        public bool IsPrimer => _isPrimer;
        public bool IsObjective => _isObjective;
        bool IsPrimed { get; set; }

        void Start () {
            _collection = GameController.Instance.Explodables;
            _collection.Add(this);
        }

        void OnDestroy () {
            _collection.Remove(this);
        }

        public void Explode () {
            StartCoroutine(ExplosionLoop());
        }

        // @TODO Break off the animation implementation to a separate class so Nathan can hook in his own animations without dealing with the DoTween fluff
        IEnumerator ExplosionLoop () {
            // Prevent recursive detonations by setting the primed flag
            IsPrimed = true;

            // Track this globally so we can wait for all explosions to resolve
            _collection.AddExploding(this);

            // Let the character start priming before the explosion
            yield return new WaitForSeconds(_explosionDelay);

            // Run the explode animation with dotween bounce
            _explosionGraphic.SetActive(true);
            var targetScale = _explosionGraphic.transform.localScale;
            _explosionGraphic.transform.localScale = Vector3.zero;
            var tween = _explosionGraphic.transform
                .DOScale(targetScale, _explosionDuration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

            // Animation ends (or at least height of explosion where next explosions should trigger)
            yield return tween.WaitForCompletion();

            // Find all explodables inside the explosion area
            foreach (var target in _explosionTrigger.TrackedObjects) {
                if (!target.IsPrimed) {
                    target.Explode();
                }
            }

            // Inform the game state that this explosion has resolved
            _collection.RemoveExploding(this);

            // Destroy this since it has exploded
            // @NOTE This is implementation specific, do not put in base class
            Destroy(gameObject);
        }
    }
}
