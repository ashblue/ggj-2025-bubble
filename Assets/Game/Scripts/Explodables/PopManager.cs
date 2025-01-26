using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using GameJammers.GGJ2025.FloppyDisks;

namespace GameJammers.GGJ2025.Explodables
{
    public class PopManager : MonoBehaviour
    {
        public static PopManager Instance;

        public VisualEffect bubbleVfx;
        public float PopDelayMin = 0.05f;
        public float PopDelayMax = 0.3f;
        private Queue<Poppable> queuedPops;
        List<Poppable> pops;
        private float nextPopAllowedTime;
        private bool _isDone;

        // todo stop vfx if nothing comes through the queue for a time?

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            queuedPops = new Queue<Poppable>();
        }

        // Update is called once per frame
        void Update()
        {
            if (queuedPops.Count > 0 && Time.time >= nextPopAllowedTime)
            {
                var pop = queuedPops.Dequeue();
                bubbleVfx.SetVector3("SpawnPositionWs", pop.transform.position);
                bubbleVfx.SendEvent("OnPop");
                pop.Pop();
                nextPopAllowedTime = Time.time + Random.Range(PopDelayMin, PopDelayMax);
            }

            if (_isDone) {
                // done with popping
                // trigger score tally
                ScoreBoardController.Instance.Play();
            }
        }

        public void CheckDone () {
            // reported from poppables
            // after sequence, check to see if any explodables are still exploding
            if (GameController.Instance.Explodables.ItemsExploding.Count == 0) {
                _isDone = true;
            }
        }

        public void AddPopToQueue(Poppable poppable)
        {
            if (poppable.canPop && !queuedPops.Contains(poppable))
            {
                poppable.canPop = false;
                queuedPops.Enqueue(poppable);
            }
        }


        public void AddPopToQueue(Poppable[] poppables)
        {
            for (int i = 0; i < poppables.Length; i++)
            {
                AddPopToQueue(poppables[i]);
            }
        }
    }
}
