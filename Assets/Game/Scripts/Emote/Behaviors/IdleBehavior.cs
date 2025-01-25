using System.Buffers.Text;
using DG.Tweening;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class IdleBehavior : EmoteBehavior {
        public float WakeDuration;
        public Ease BlinkEaseFunction;
        int NumBlinks;
        float BlinkBaseSpeed;
        float BlinkFrequency;
        Ease EaseFunction;
        public IdleBehavior (float wakeDuration = 4f, int numBlinks = 5, float blinkFreq = 0.01f, float blinkBaseSpeed = 0.1f, Ease easeFunction = Ease.Linear, EmoteBehavior parent = null) : base(parent) {
            NumBlinks = numBlinks;
            BlinkBaseSpeed = blinkBaseSpeed;
            EaseFunction = easeFunction;
            BlinkFrequency = blinkFreq;
            WakeDuration = wakeDuration;
            BlinkEaseFunction = easeFunction;
        }

        public override Sequence BuildSequence () {
            emoteSequence = DOTween.Sequence();
            TransformMask Mask = new TransformMask(true, true, true);

            var neutralPose = eyePoseRegistry.GetPose("Neutral");

            // wake
            var wake = new Wake(WakeDuration) {
                targetTransform = this.targetTransform,
                LeftEye = this.LeftEye,
                RightEye = this.RightEye,
            };
            emoteSequence.Append(wake.BuildSequence());

            // fast blink
            var blink = new Blink(neutralPose, 3, blinkFrequency:5f, blinkBaseSpeed:0.05f, ease:BlinkEaseFunction) {
                targetTransform = this.targetTransform,
                LeftEye = this.LeftEye,
                RightEye = this.RightEye,
            };
            emoteSequence.Append(blink.BuildSequence());

            // slow blink for ~10s
            var blinkSlow = new Blink(neutralPose, 4, blinkFrequency:0.5f, blinkRandomRange:0.5f, ease:BlinkEaseFunction) {
                targetTransform = this.targetTransform,
                LeftEye = this.LeftEye,
                RightEye = this.RightEye,
            };
            emoteSequence.Append(blinkSlow.BuildSequence());
            emoteSequence.AppendInterval(1f);

            // suspicious
            emoteSequence.Append(new Suspicious(parent:this).BuildSequence());

            // back to neutral and blink
            var toNeutral = new ToPose("Neutral", 0.5f, parent: this);
            emoteSequence.Append(toNeutral.BuildSequence());

            emoteSequence.Append(blinkSlow.BuildSequence());



            return emoteSequence;
        }
    }
}
