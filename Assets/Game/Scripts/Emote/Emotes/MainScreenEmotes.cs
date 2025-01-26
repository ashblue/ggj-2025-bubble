using DG.Tweening;
using GameJammers.GGJ2025.Emote.Behaviors;
using GameJammers.GGJ2025.Explodables;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Emotes {
    public class MainScreenEmotes : EmoteSystem {

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
            Behaviors.Add((0.1f, new ToPose("Tired", 6f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Left", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Up", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Down", 2f, parent:asleep_awake)));
            Behaviors.Add((0.1f, new ToPose("Look_Right", 2f, parent:asleep_awake)));



        }

        public override void Update () {
            // rarely, kick off the swing animation
            if (!_stopped && (_currentSequence == null || !_currentSequence.IsPlaying())) {
                if (Random.value < 0.05f) {
                    var behavior = new Blink(_neutralEyePose, 4) {
                        targetTransform = this.transform,
                        LeftEye = this.LeftEye,
                        RightEye = this.RightEye,
                    };
                    animator.SetTrigger("Swing");
                }
                else {
                    var nextBehavior = NextBehavior();
                    Debug.Log($"Next behavior: {nextBehavior.GetType()}");
                    _currentSequence = nextBehavior.BuildSequence();
                }

            }
        }
    }
}
