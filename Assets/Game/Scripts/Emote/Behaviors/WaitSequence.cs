using DG.Tweening;
using UnityEngine;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class WaitSequence : EmoteBehavior {
        public float Duration;
        public WaitSequence (float duration = 1f, EmoteBehavior parent = null) : base(parent:parent) {
            Duration = duration;
        }

        public override Sequence BuildSequence () {
            emoteSequence = DOTween.Sequence ();
            emoteSequence.AppendInterval(Duration);
            emoteSequence.AppendCallback(() => Debug.Log("Wait sequence finished")); // have to do something or no sequence // could not replace this with an empty or with a call that toggled a bool
            return emoteSequence;
        }
    }
}
