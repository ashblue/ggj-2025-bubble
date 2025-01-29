using DG.Tweening;
using GameJammers.GGJ2025.Emote.Behaviors;
using GameJammers.GGJ2025.Explodables;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Emotes {
    public class MainScreenEmotes : EmoteSystem {
        bool isWaiting = false;

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
            Behaviors.Add((0.2f, new Suspicious(parent:asleep_awake))); // passing as parent to duplicate parameters
            Behaviors.Add((0.6f, new Blink(_neutralEyePose, Random.Range(1,4), parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Diff_Size", 0.4f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Tired", 6f, parent:asleep_awake, easeFunction: Ease.OutBounce)));
            Behaviors.Add((0.1f, new ToPose("Look_Left", 0.3f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Up", 0.3f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Down", 0.3f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Right", 0.3f, parent:asleep_awake)));

            PassTargetsToBehaviors();

            Debug.Log("Behaviors");
            Behaviors.ForEach(sel => Debug.Log($"weight: {sel.weight}: {sel.behavior.GetType()}"));
        }

        public override void Update () {
            // rarely, kick off the swing animation
            if (!CanStartNextSequence()) return;
            if (isWaiting) {
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
            else {
                var wait = new WaitSequence(Random.Range(1f,3f)) {targetTransform = this.transform, LeftEye = this.LeftEye, RightEye = this.RightEye };
                _currentSequence = wait.BuildSequence();
            }
            isWaiting = !isWaiting;
        }
    }
}
