using DG.Tweening;
using GameJammers.GGJ2025.Emote.Behaviors;
using GameJammers.GGJ2025.Explodables;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Emotes {
    public class FriendEmotes : EmoteSystem {

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

            Behaviors.Add((0.2f, asleep_awake));
            Behaviors.Add((0.6f, new Blink(_neutralEyePose, Random.Range(1,4), parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Tired", 0.4f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Tired", 6f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Left", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Up", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Down", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Right", 2f, parent:asleep_awake)));

            PassTargetsToBehaviors(); // somewhat redundant with parent being set above, but prevents any missed behaviors

        }

        public override void Update () {
            // rarely, kick off the swing animation
            if (!CanStartNextSequence()) return;
            if (Random.value < 0.05f) {
                var behavior = new Blink(_neutralEyePose, 4) {
                    targetTransform = this.transform,
                    LeftEye = this.LeftEye,
                    RightEye = this.RightEye,
                };
                animator.SetTrigger("Swing");
                _currentSequence = behavior.BuildSequence();
            }
            else {
                var nextBehavior = NextBehavior();
                _currentSequence = nextBehavior.BuildSequence();
            }
        }
    }
}
