using DG.Tweening;
using UnityEngine.UI;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class Wake : EmoteBehavior {
        public float Duration;

        public Wake (float duration = 2f, EmoteBehavior parent = null) : base(parent) {
            Duration = duration;
        }

        public override Sequence BuildSequence () {
            SequenceInfo = new EmoteSequenceInfo();
            var toSleepTime = 0.05f;
            var waitTime = Duration * 0.5f;
            var remainingTime = Duration - waitTime - toSleepTime;

            var sleepPose = eyePoseRegistry.GetPose("Large_Eyes");
            var neutralPose = eyePoseRegistry.GetPose("Neutral");

            var mask = new TransformMask();

            SequenceInfo.eyePoses.Add((0, toSleepTime, sleepPose, mask, Ease.Linear));
            SequenceInfo.eyePoses.Add((toSleepTime + waitTime, remainingTime, neutralPose, mask, Ease.InBounce));

            return base.BuildSequence();
        }
    }
}
