using System;
using System.Collections;
using System.Collections.Generic;
using GameJammers.GGJ2025.Explodables;
using GameJammers.GGJ2025.FloppyDisks;
using GameJammers.GGJ2025.Utilities;
using UnityEngine;

namespace GameJammers.GGJ2025.Bootstraps {
    [Serializable]
    public class ExplodeController {
        GameController _game;
        ExplodableCollection _explodables;
        ICoroutineRunner _runner;

        // @TODO Move to class initialization and ditch serializable if there is no need for it
        public void Setup (GameController game, ExplodableCollection explodables, ICoroutineRunner runner) {
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
            foreach (var explode in _explodables.Items) {
                if (explode.IsPrimer) {
                    primers.Add(explode);
                }
            }

            _runner.StartCoroutine(ChainReaction(primers));
        }

        IEnumerator ChainReaction (List<IExplodable> primers) {
            // Detonate all primers
            foreach (var item in primers) {
                item.Explode();
            }

            // Wait for all explosions to have been resolved
            while (_explodables.ItemsExploding.Count > 0) {
                yield return null;
            }

            // Send off the results to the game score board for tallying
            // @TODO Game state change should be on score board
            // @TODO Tally up RAM score (total disks used), total objectives, and remaining objectives
            // _game.SetState(GameState.Scoring);
            // _game.ScoreBoard.Play();

            CursorInteractController.Instance.Unlock();

            Debug.Log("Done exploding");
        }
    }
}
