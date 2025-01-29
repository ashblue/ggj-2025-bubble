using System;
using GameJammers.GGJ2025.Explodables;
using GameJammers.GGJ2025.Emote.Behaviors;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;

namespace GameJammers.GGJ2025.Emote.Emotes {
    public class TestEmoteBehavior : EmoteSystem {
        public override void Awake () {
            Behaviors = new List<(float weight, EmoteBehavior behavior)>();

            EmoteBehavior suspicious = new Suspicious();

            Behaviors.Add((1f,suspicious));
            Behaviors.Add((1f, new WaitSequence(3f)));

            PassTargetsToBehaviors();
        }
    }
}
