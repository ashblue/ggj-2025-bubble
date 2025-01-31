﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameJammers.GGJ2025.Explodables;
using GameJammers.GGJ2025.Utilities;

namespace GameJammers.GGJ2025.FloppyDisks {
    [Serializable]
    public class ExplodeController {
        GameController _game;
        ExplodableCollection _explodables;
        ICoroutineRunner _runner;

        public ExplodeController (GameController game, ExplodableCollection explodables, ICoroutineRunner runner) {
            _game = game;
            _explodables = explodables;
            _runner = runner;
        }

        public void Begin () {
            if (_game.State != GameState.Placement) return;

            _game.SetState(GameState.Explosion);

            // Disable all disk interactions so we can't screw with the game state
            CursorInteractController.Instance.Lock();

            // Get all of the primers
            var primers = new List<IExplodable>();
            // not handled here now
            //var objectives = 0;
            foreach (var explode in _explodables.Items) {
                //if (explode.IsObjective) {
                //    objectives++;
                //}

                if (explode.AutoExplode) {
                    primers.Add(explode);

                }
            }

            foreach (var primer in primers) {
                primer.Explode();
            }

            //_runner.StartCoroutine(ChainReaction(primers, objectives));
        }

        /*IEnumerator ChainReaction (List<IExplodable> primers, int objectives) {
            // Detonate all primers
            foreach (var item in primers) {
                item.Explode();
            }

            // Wait for all explosions to have been resolved
            while (_explodables.ItemsExploding.Count > 0) {
                yield return null;
            }

            // Count the number of objectives that didn't explode
            var objectivesComplete = objectives;
            foreach (var item in _explodables.Items) {
                if (item.IsObjective && !item.IsPrimed) {
                    objectivesComplete--;
                }
            }

            // Report the final level score to the score board
            // get from scoreboard var ramMax = _game.Ram.Max;
            // get from scoreboard var ramScore = _game.Ram.Current;
            // handled by pop manager now ScoreBoardController.Instance.Play(objectives, objectivesComplete, ramMax, ramScore);
        }*/
    }
}
