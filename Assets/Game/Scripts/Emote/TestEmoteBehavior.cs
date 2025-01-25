using System;
using GameJammers.GGJ2025.Bubble;
using GameJammers.GGJ2025.Emote.Behaviors;
using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace GameJammers.GGJ2025.Emote {
    [RequireComponent(typeof(Poppable))]
    public class TestEmoteBehavior : MonoBehaviour {
        public Poppable poppable;
        public Ease ease;
        public EmoteBehavior BehaviorToTest;
        //IdleBehavior IdleBehavior;
        EmoteBehavior BaseBehavior;

        public Transform leftEye, rightEye;

        public bool Play;
        bool playingTestBehavior;

        void Start () {
            SetEyeTransforms();
            EyePoseRegistry eyePoseRegistry = EyePoseRegistryLoader.LoadRegistry();
            var neutralPose = eyePoseRegistry.GetPose("Neutral");
            //IdleBehavior = new IdleBehavior() {
            //IdleBehavior = new SingleBlink(neutralPose, duration: 0.5f){
            BaseBehavior = new ToPose("Large_Eyes", 1f){
                targetTransform = transform,
                LeftEye = leftEye,
                RightEye = rightEye,
            };

            //BehaviorToTest = new Blink(neutralPose, 2, blinkBaseSpeed:0.1f, blinkFrequency:4f, ease:ease){
            BehaviorToTest = new IdleBehavior(blinkBaseSpeed: 0.1f) {
            //BehaviorToTest = new SingleBlink(neutralPose, blinkSpeed) {
            //BehaviorToTest = new Wake(4f){
            //BehaviorToTest = new ToPose("Large_Eyes", 0.3f){
                targetTransform = transform,
                LeftEye = leftEye,
                RightEye = rightEye,
            };

            BaseBehavior.BuildSequence();
        }

        //[ExecuteAlways]
        void Update () {
            if (leftEye == null || rightEye == null) {
                SetEyeTransforms();
            }
            EyePoseRegistry eyePoseRegistry = EyePoseRegistryLoader.LoadRegistry();
            var neutralPose = eyePoseRegistry.GetPose("Neutral");



            if (Play && poppable != null && BehaviorToTest != null) {
                // in either case, ensure the other is killed and
                EmoteBehavior currentBehavior = playingTestBehavior ? BehaviorToTest : BaseBehavior;
                EmoteBehavior nextBehavior = playingTestBehavior ? BaseBehavior : BehaviorToTest;

                if (currentBehavior.IsSequencePlaying()) {
                    float elapsed = currentBehavior.emoteSequence.Elapsed(); // Time elapsed
                    float totalDuration = currentBehavior.emoteSequence.Duration(); // Total sequence duration
                    float progress = currentBehavior.emoteSequence.ElapsedPercentage(); // Percentage of completion

                    Debug.Log($"{currentBehavior.GetType().Name} Sequence Progress: {progress * 100:F2}% ({elapsed:F2}s / {totalDuration:F2}s)");
                }


                if (!currentBehavior.IsSequencePlaying()) {
                    Debug.Log($"Playing {nextBehavior} behavior");
                    playingTestBehavior = !playingTestBehavior;
                    Play = false;
                    var currSeq = nextBehavior.BuildSequence();
                    currSeq.AppendInterval(3f).InsertCallback(0.1f,() => SetPlay(true)); // could assign and play with pausing in another life
                    Debug.Log($"Tween duration: {currSeq.Duration()}");
                }
            }
        }

        void SetPlay (bool x) {
            Play = x;
        }

        void SetEyeTransforms () {
            // Find the eyes in the selected object's children
            string leftEyeSuffix = "Eye_Left";
            string rightEyeSuffix = "Eye_Right";
            var children = transform.GetComponentsInChildren<Transform>();
            leftEye = children.FirstOrDefault(wh => wh.name.EndsWith(leftEyeSuffix));
            rightEye = children.FirstOrDefault(wh => wh.name.EndsWith(rightEyeSuffix));
        }
    }
}
