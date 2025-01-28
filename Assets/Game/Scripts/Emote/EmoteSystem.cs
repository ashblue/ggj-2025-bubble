using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameJammers.GGJ2025.Emote.Behaviors;
using DG.Tweening;
using GameJammers.GGJ2025.Explodables;
using Random = UnityEngine.Random;

namespace GameJammers.GGJ2025.Emote {
    public abstract class EmoteSystem : MonoBehaviour {
        [NonSerialized] public List<(float weight, EmoteBehavior behavior)> Behaviors;
        public Transform LeftEye, RightEye;
        public Animator animator;
        public float WaitWeight = 20f;


        protected Sequence _currentSequence;
        protected bool _stopped = false;

        public virtual void Awake () {
            Behaviors = new List<(float, EmoteBehavior)>() {
                (WaitWeight, new WaitSequence(5f) {targetTransform = transform, LeftEye = this.LeftEye, RightEye = this.RightEye})
            };
            PassTargetsToBehaviors();
        }

        protected void PassTargetsToBehaviors () {
            // meant to be called after awake
            Behaviors.ForEach(sel => {
                sel.behavior.targetTransform = this.transform;
                sel.behavior.LeftEye = this.LeftEye;
                sel.behavior.RightEye = this.RightEye;
            });
        }

        public virtual void Update () {
            if (!CanStartNextSequence()) return;
            var behavior = NextBehavior();
            _currentSequence = behavior.BuildSequence();
        }

        public EmoteBehavior NextBehavior () {
            var totWeight = Behaviors.Sum(sel => sel.weight);
            var choice = Random.value * totWeight;
            float w = 0;
            for (int i = 0; i < Behaviors.Count; i++) {
                w += Behaviors[i].weight;
                if (w >= choice) {
                    return Behaviors[i].behavior;
                }
            }

            return null;
        }

        public bool CanStartNextSequence () {
            return !_stopped && (_currentSequence == null || !_currentSequence.IsActive() ||
                                 _currentSequence.IsComplete());
        }

        public void ForceBehavior (EmoteBehavior behavior) {
            KillBehaviors();
            _currentSequence = behavior.BuildSequence();
        }

        public void KillBehaviors () {
            _stopped = true;
            _currentSequence.Kill();
        }

        public virtual void HandlePop () {
            ForceBehavior(GetDeadPose());
        }

        public virtual EmoteBehavior GetDeadPose () {
            return new ToPose("X", 0.1f) {
                targetTransform = transform,
                LeftEye = this.LeftEye,
                RightEye = this.RightEye
            };
        }
    }
}
