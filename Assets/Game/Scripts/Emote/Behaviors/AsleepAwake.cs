using DG.Tweening;
using GameJammers.GGJ2025.Explodables;
using UnityEngine;
using UnityEngine.UI;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    [RequireComponent(typeof(Poppable))]
    public class AsleepAwake : EmoteBehavior {

        public AsleepAwake (EmoteBehavior parent = null) : base(parent) {
        }

        public override Sequence BuildSequence () {
            Sequence asleep_awake = DOTween.Sequence ();
            asleep_awake.Append(new Sleep(parent: this).BuildSequence());
            asleep_awake.AppendInterval(7f);
            asleep_awake.Append(new Wake(parent:this).BuildSequence());
            return asleep_awake;
        }
    }
}
