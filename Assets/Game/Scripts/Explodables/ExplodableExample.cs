using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GameJammers.GGJ2025.Explodables {
    public class ExplodableExample : ExplodableBase {
        bool _ready;

        [Header("Animation")]

        [SerializeField]
        float _explosionDelay = 0.5f;

        [SerializeField]
        GameObject _explosionGraphic;

        [SerializeField]
        float _explosionDuration = 0.5f;

        protected override void OnPlayAnimation () {
            // Implement your own custom animation hook here
            StartCoroutine(PlayLoop());
        }

        IEnumerator PlayLoop () {
            _explosionGraphic.SetActive(true);
            var targetScale = _explosionGraphic.transform.localScale;
            _explosionGraphic.transform.localScale = Vector3.zero;

            var tween = _explosionGraphic.transform
                .DOScale(targetScale, _explosionDuration)
                .SetDelay(_explosionDelay)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);

            yield return tween.WaitForCompletion();

            _ready = true;
        }

        protected override bool GetIsAnimationComplete () {
            // When this is done it informs the objects hit by the explosion to explode
            return _ready;
        }

        protected override void OnExplosionComplete () {
            // This hook triggers after the hit objects have been exploded
            Destroy(gameObject);
        }

        protected override void OnDestroy () {
            base.OnDestroy();

            _explosionGraphic.transform.DOKill();
        }
    }
}
