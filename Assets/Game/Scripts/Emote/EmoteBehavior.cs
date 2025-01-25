using UnityEngine;

using System.Collections.Generic;

using DG.Tweening;

namespace GameJammers.GGJ2025.Emote {
    public enum EmoteType {
        Happy,
        Sad,
        Angry,
        Surprised,
        Neutral
    }

    public enum EyeMask {
        Left,
        Right,
        Both
    }

    public class TransformMask {
        public (bool location, bool rotation, bool scale, EyeMask eyes) Mask;

        public TransformMask (bool usePosition = true, bool useRotation = true, bool useScale = true, EyeMask eyes = EyeMask.Both) {
            Mask = (usePosition, useRotation, useScale, eyes);
        }

        public bool Any () {
            return Mask.location || Mask.rotation || Mask.scale;
        }

        public bool LeftEye () {
            return Mask.eyes is EyeMask.Both or EyeMask.Left;
        }

        public bool RightEye () {
            return Mask.eyes is EyeMask.Both or EyeMask.Right;
        }
    }

    public class EmoteBehavior {
        public Sequence emoteSequence;
        public EmoteType emoteType;
        public Transform targetTransform, LeftEye, RightEye;
        public EyePoseRegistry eyePoseRegistry;

        public EmoteSequenceInfo SequenceInfo; // going to make this for the generic case, but for now will likely override and not use this in our more complex scenarios (handy if we had a custom timeline editor though)

        public EmoteBehavior (EmoteBehavior parent = null) {
            //emoteSequence = DOTween.Sequence ();
            emoteType = EmoteType.Happy;
            SequenceInfo = new EmoteSequenceInfo ();
            eyePoseRegistry = EyePoseRegistryLoader.LoadRegistry();

            if (parent != null) {
                this.targetTransform = parent.targetTransform;
                this.LeftEye = parent.LeftEye;
                this.RightEye = parent.RightEye;
            }
        }

        public virtual Sequence BuildSequence () {
            emoteSequence = DOTween.Sequence ();
            // handle transformations
            //SequenceInfo.transformations.Where(wh => wh.mask.Any()).ToList().ForEach(sel => emoteSequence.Insert(sel.start, TweenTransform(targetTransform, sel)));
            for (int i = 0; i < SequenceInfo.transformations.Count; i++) {
                var transformation = SequenceInfo.transformations[i];
                //Debug.Log($"Processing Transformation: Start={transformation.start}, Duration={transformation.duration}, Mask={transformation.mask.Any()}");
                if (transformation.mask.Any()) {
                    emoteSequence.Insert(transformation.start, TweenTransform(targetTransform, transformation));
                }
            }
            // handle eye poses
            if (LeftEye == null) {
                Debug.LogWarning("Left Eye not found");
            }

            if (RightEye == null) {
                Debug.LogWarning("Right Eye not found");
            }
            //SequenceInfo.eyePoses.Where(wh => wh.mask.Any()).ToList().ForEach(sel => emoteSequence.Insert(sel.start, TweenEyePose(sel)));
            for (int i = 0; i < SequenceInfo.eyePoses.Count; i++) {
                var pose = SequenceInfo.eyePoses[i];
                //Debug.Log($"Processing Transformation: Start={pose.start}, Duration={pose.duration}, Mask={pose.mask.Any()}");

                if (pose.mask.Any()) {
                    emoteSequence.Insert(pose.start, TweenEyePose(pose));
                }
            }

            // handle other behaviors
            //SequenceInfo.additionalBehaviors.ForEach(sel => emoteSequence.Insert(sel.start, sel.emoteBehavior.BuildSequence()));
            for (int i = 0; i < SequenceInfo.additionalBehaviors.Count; i++) {
                var behavior = SequenceInfo.additionalBehaviors[i];
                emoteSequence.Insert(behavior.start, behavior.emoteBehavior.BuildSequence());
            }

            return emoteSequence;
        }

        protected static Sequence TweenTransform (Transform target, (float start, float duration, TransformInfo otherT, TransformMask mask, Ease ease) transformation) {
            var seq = DOTween.Sequence ();
            if (transformation.mask.Mask.location) {
                Transform t1 = target; // some dotween issue with foreach https://dotween.demigiant.com/support.php?faq=Tweens%20and%20callback%20targets
                seq.Insert(0, t1.DOLocalMove(transformation.otherT.LocalPosition, transformation.duration).SetEase(transformation.ease));
            }
            if (transformation.mask.Mask.rotation) {
                Transform t2 = target; // some dotween issue with foreach https://dotween.demigiant.com/support.php?faq=Tweens%20and%20callback%20targets
                seq.Insert(0, t2.DOLocalRotate(transformation.otherT.LocalRotation.eulerAngles, transformation.duration).SetEase(transformation.ease));
            }

            if (transformation.mask.Mask.scale) {
                //Debug.Log($"Tweening scale from {target.localScale} to {transformation.otherT.LocalScale}");
                Transform t3 = target; // some dotween issue with foreach https://dotween.demigiant.com/support.php?faq=Tweens%20and%20callback%20targets
                seq.Insert(0, t3.DOScale(transformation.otherT.LocalScale, transformation.duration).SetEase(transformation.ease));
            }

            return seq;
        }

        protected Sequence TweenEyePose (
            (float start, float duration, EyePose eyePose, TransformMask mask, Ease ease) transformation) {
            // note start is handled before this (probably won't change)
            //Debug.Log($"Tweening pose on {LeftEye.name} to {transformation.eyePose.name} at {transformation.start} for {transformation.duration}");
            var seq = DOTween.Sequence ();
            if (transformation.mask.LeftEye()) {
                seq.Insert(0, TweenTransform(LeftEye, (transformation.start, transformation.duration, transformation.eyePose.left, transformation.mask, transformation.ease)));
            }
            if (transformation.mask.RightEye()) {
                seq.Insert(0, TweenTransform(RightEye, (transformation.start, transformation.duration, transformation.eyePose.right, transformation.mask, transformation.ease)));
            }
            return seq;
        }

        public bool IsSequencePlaying () {
            return emoteSequence != null && emoteSequence.IsPlaying();
        }

        public void KillSequence () {
            if (IsSequencePlaying()) {
                emoteSequence.Kill();
            }
        }

    }

    public class EmoteSequenceInfo {
        public List<(float start, float duration, TransformInfo t, TransformMask mask, Ease ease)> transformations;
        public List<(float start, float duration, EyePose eyePose, TransformMask mask, Ease ease)> eyePoses;
        public List<(float start, EmoteBehavior emoteBehavior)> additionalBehaviors;

        public EmoteSequenceInfo () {
            transformations = new List<(float, float, TransformInfo, TransformMask, Ease)> ();
            eyePoses = new List<(float, float, EyePose, TransformMask, Ease)> ();
            additionalBehaviors = new List<(float, EmoteBehavior)> ();
        }
    }
}
