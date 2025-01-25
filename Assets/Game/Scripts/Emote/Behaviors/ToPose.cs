using DG.Tweening;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class ToPose : EmoteBehavior {
        public EyePose EyePose;
        public float Duration;
        public Ease EaseFunction;
        TransformMask Mask;

        public ToPose (string EyePoseName, float duration, TransformMask mask = null, Ease easeFunction = Ease.Linear, EmoteBehavior parent = null) : base(parent) {
            EyePose = eyePoseRegistry.GetPose(EyePoseName);
            Duration = duration;
            EaseFunction = easeFunction;
            Mask = mask ?? new TransformMask();
        }

        public override Sequence BuildSequence () {
            SequenceInfo = new EmoteSequenceInfo();
            SequenceInfo.eyePoses.Add((0, Duration, EyePose, Mask, EaseFunction));
            return base.BuildSequence();
        }
    }
}
