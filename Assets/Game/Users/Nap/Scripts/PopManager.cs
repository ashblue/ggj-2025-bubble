using System.Collections.Generic;

using UnityEngine;
using UnityEngine.VFX;

namespace GameJammers.GGJ2025.Bubble
{
    public class PopManager : MonoBehaviour
    {
        public static PopManager Instance;

        public VisualEffect bubbleVfx;
        public float PopDelayMin = 0.01f;
        public float PopDelayMax = 0.3f;
        private Queue<Poppable> queuedPops;
        private float nextPopAllowedTime;
        
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
