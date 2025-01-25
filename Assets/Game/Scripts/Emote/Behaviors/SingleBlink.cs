using DG.Tweening;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class SingleBlink : EmoteBehavior {
        public TransformMask Mask;
        public Ease EaseFunction;

        public float Duration;

        EyePose _returnPose;

        public SingleBlink (EyePose returnPose, float duration = 0.1f, TransformMask mask = null, Ease ease = Ease.Linear, EmoteBehavior parent = null) : base(parent) {
            Duration = duration;
            Mask = mask ?? new TransformMask(false, false, true);
            EaseFunction = ease;
            _returnPose = returnPose;
        }

        public override Sequence BuildSequence () {
            SequenceInfo = new EmoteSequenceInfo();
            var blinkPose = eyePoseRegistry.GetPose("Squint");

            SequenceInfo.eyePoses.Add((0, Duration, blinkPose, Mask, EaseFunction));
            SequenceInfo.eyePoses.Add((Duration, Duration, _returnPose, Mask, EaseFunction));
            return base.BuildSequence ();
        }
    }
}
