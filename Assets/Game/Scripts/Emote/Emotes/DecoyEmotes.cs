using DG.Tweening;
using GameJammers.GGJ2025.Emote.Behaviors;
using GameJammers.GGJ2025.Explodables;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Emotes {
    public class DecoyEmotes : EmoteSystem {

        private EyePose _neutralEyePose;
        public override void Awake () {
            base.Awake ();

            EmoteBehavior asleep_awake = new AsleepAwake() {
                targetTransform = this.transform,
                LeftEye = this.LeftEye,
                RightEye = this.RightEye,
            };
            var eyePoseRegistry = EyePoseRegistryLoader.LoadRegistry();
            _neutralEyePose = eyePoseRegistry.GetPose("Neutral");

            Behaviors.Add((0.4f, new Suspicious(parent:asleep_awake))); // passing as parent to duplicate parameters
            Behaviors.Add((0.6f, new Blink(_neutralEyePose, Random.Range(1,4), blinkRandomRange: 0.5f, parent:asleep_awake)));
            Behaviors.Add((1f, new ToPose("Diff_Size", 0.4f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Tired", 6f, parent:asleep_awake)));
            Behaviors.Add((0.2f, new ToPose("Look_Left", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Up", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Down", 2f, parent:asleep_awake)));
            Behaviors.Add((0.2f, new ToPose("Look_Right", 2f, parent:asleep_awake)));
        }
        public override EmoteBehavior GetDeadPose () {
            var behavior = new EmoteBehavior() {
                targetTransform = this.transform,
                LeftEye = this.LeftEye,
                RightEye = this.RightEye,
            };
            var eyePoseRegistry = EyePoseRegistryLoader.LoadRegistry();

            var rand_mask = Random.Range(0, 3);

            behavior.SequenceInfo.eyePoses.Add((0, 0.1f, eyePoseRegistry.GetPose("Diff_Size"), new TransformMask(false, false, true, (EyeMask)rand_mask), Ease.Linear));
            behavior.SequenceInfo.eyePoses.Add((0, 0.1f, eyePoseRegistry.GetPose("X"), new TransformMask(true, true, false), Ease.Linear));

            return behavior;
        }
    }
}
