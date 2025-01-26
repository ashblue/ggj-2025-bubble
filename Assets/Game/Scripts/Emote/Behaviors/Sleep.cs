using DG.Tweening;
using GameJammers.GGJ2025.Explodables;
using UnityEngine;
using UnityEngine.UI;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    [RequireComponent(typeof(Poppable))]
    public class Sleep : EmoteBehavior {
        public float Duration;

        public Sleep (float duration = 4f, EmoteBehavior parent = null) : base(parent) {
            Duration = duration;
        }

        public override Sequence BuildSequence () {
            SequenceInfo = new EmoteSequenceInfo();
            var toNeutral = 0.05f;
            var waitTime = Duration * 0.25f;
            var remainingTime = Duration - waitTime - toNeutral;

            var sleepPose = eyePoseRegistry.GetPose("Large_Eyes");
            var neutralPose = eyePoseRegistry.GetPose("Neutral");

            var mask = new TransformMask();

            SequenceInfo.eyePoses.Add((0, toNeutral, neutralPose, mask, Ease.Linear));
            SequenceInfo.eyePoses.Add((toNeutral + waitTime, remainingTime, sleepPose, mask, Ease.OutBounce));

            return base.BuildSequence();
        }
    }
}
