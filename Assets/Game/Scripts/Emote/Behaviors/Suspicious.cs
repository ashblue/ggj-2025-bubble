using DG.Tweening;

namespace GameJammers.GGJ2025.Emote.Behaviors {
    public class Suspicious : EmoteBehavior {
        public Suspicious (EmoteBehavior parent) : base(parent) {

        }

        public override Sequence BuildSequence () {
            emoteSequence = DOTween.Sequence();

            // squint eyes and look around
            var squintStart = emoteSequence.Duration();
            var squint = new ToPose("Squint", 0.5f, new TransformMask(false,false, true), Ease.InQuad, parent: this);
            emoteSequence.Append(squint.BuildSequence());
            emoteSequence.AppendInterval(0.6f);
            var lookRight = new ToPose("Look_Right", 0.3f, new TransformMask(true, false, false), Ease.OutCubic, parent: this);
            emoteSequence.Append(lookRight.BuildSequence());
            emoteSequence.AppendInterval(0.5f);
            var lookLeft = new ToPose("Look_Left", 0.3f, new TransformMask(true, false, false), Ease.OutCubic, parent: this);
            emoteSequence.Append(lookLeft.BuildSequence());
            emoteSequence.AppendInterval(0.8f);

            return emoteSequence;
        }
    }
}
