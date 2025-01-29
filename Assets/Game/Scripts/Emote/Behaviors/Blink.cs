using DG.Tweening;
using UnityEngine;
namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class Blink : EmoteBehavior {
        public TransformMask Mask;
        public Ease EaseFunction;

        int NumBlinks;
        float BlinkBaseSpeed;
        float BlinkFrequency;
        float BlinkRandomRange; // must be 0-1

        EyePose _returnPose;

        public Blink (EyePose returnPose, int numBlinks, float blinkBaseSpeed = 0.1f, float blinkFrequency = 0.25f, float blinkRandomRange = 0.2f, TransformMask mask = null, Ease ease = Ease.Linear, EmoteBehavior parent = null) : base(parent) {
            NumBlinks = numBlinks;
            BlinkBaseSpeed = blinkBaseSpeed;
            BlinkFrequency = blinkFrequency;
            Mask = mask ?? new TransformMask(false, false, true);
            EaseFunction = ease;
            _returnPose = returnPose;
            BlinkRandomRange = Mathf.Clamp01(blinkRandomRange);
        }

        public override Sequence BuildSequence () {
            SequenceInfo = new EmoteSequenceInfo(); // move logic out of here and make protected so children can call
            var waitBaseTime = 1 / BlinkFrequency;
            var neutralPose = eyePoseRegistry.GetPose("Neutral"); // could call once when creating? Have to ensure these can't be edited downstream
            float startTime = 0;
            for (int i = 0; i < NumBlinks; i++) {
                var singleBlink = new SingleBlink(
                    returnPose: neutralPose,
                    duration: Random.Range(BlinkBaseSpeed * 0.5f, BlinkBaseSpeed * 1.5f),
                    ease: EaseFunction,
                    parent: this
                );
                // neutral for random seconds
                SequenceInfo.additionalBehaviors.Add((startTime, singleBlink));
                // blink
                // neutral for random seconds
                startTime += waitBaseTime * Random.Range(1-BlinkRandomRange, 1+BlinkRandomRange) + BlinkBaseSpeed;
                // add in movement later
            }


            return base.BuildSequence ();
        }
    }
}
