using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using GameJammers.GGJ2025.FloppyDisks;
using UnityEditor.Build.Content;

namespace GameJammers.GGJ2025.Explodables
{
    [RequireComponent(typeof(SphereCollider))]
    public class Poppable : ExplodableBase
    {
        [SerializeField] private GameObject BubbleTop;
        [SerializeField] private GameObject BubbleBottom;
        [FormerlySerializedAs("BubbleLeftArm")] [SerializeField] private GameObject BubbleArms;
        [SerializeField] private GameObject Body;
        [SerializeField] private GameObject DistortionArea;

        public float PostPopImpulseForce = 5f;
        public float PopRadius = 5f;
        public float popDelay = 0.05f;
        public LayerMask PoppableLayerMask;


        private Material BubbleTopMat;
        private Material BubbleBottomMat;
        private Material BubbleArmsMat;
        private Material DistortionAreaMat;

        private Rigidbody bodyRbody;
        private BoxCollider bodyCollider;

        private SphereCollider popSphereCollider;

        public GameObject ExplosionHighlightPrefab;

        [NonSerialized] public GameObject ExplosionHighlight;

        private Sequence popSequence;
        public bool canPop = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Start()
        {
            bodyRbody = Body.GetComponent<Rigidbody>();
            bodyCollider = Body.GetComponent<BoxCollider>();
            BubbleTopMat = BubbleTop.GetComponent<Renderer>().material;
            BubbleBottomMat = BubbleBottom.GetComponent<Renderer>().material;
            BubbleArmsMat = BubbleArms.GetComponent<Renderer>().material;
            DistortionAreaMat = DistortionArea.GetComponent<Renderer>().material;

            popSphereCollider = GetComponent<SphereCollider>();

            // highlight sphere and set radius
            ExplosionHighlight = Instantiate(ExplosionHighlightPrefab, transform);
            ExplosionHighlight.transform.localScale *= PopRadius;
            ExplosionHighlight.SetActive(false); // highlight hooks to add


            //popSequence = DOTween.Sequence();

            base.Start();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Pop()
        {
            popSequence = DOTween.Sequence();
            float popDuration = 0.5f;
            float popStart = popDuration * 0.5f;
            popSequence.Insert(popStart, BubbleTopMat.DOFloat(1, "_PopStep", popDuration));
            popSequence.Insert(popStart, BubbleBottomMat.DOFloat(1, "_PopStep", popDuration));
            popSequence.Insert(popStart, BubbleArmsMat.DOFloat(1, "_PopStep", popDuration));

            float disableTime = popStart + popDuration * 0.5f;
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleTop.gameObject.SetActive(false));
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleBottom.gameObject.SetActive(false));
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleArms.gameObject.SetActive(false));

            float distortDuration = popDuration * 0.5f;
            float distortStart = popStart * 1.5f;
            popSequence.InsertCallback(distortStart, () => DistortionArea.SetActive(true));
            popSequence.Insert(distortStart, DistortionAreaMat.DOFloat(1, "_DistortStep", distortDuration));
            popSequence.InsertCallback(distortStart, PopOthers);
            // gravity and explosion force
            popSequence.InsertCallback(disableTime, BodyPhysics);
            // add initial delay (based on bubble vfx)
            popSequence.PrependInterval(0.3f);

            // Inform the game state that this explosion has resolved
            popSequence.AppendCallback(() => GameController.Instance.Explodables.RemoveExploding(this));
            popSequence.AppendCallback(ExplosionComplete);

        }

        public void PopOthers() {
            var otherPoppables = GetPoppablesInRange();
            PopManager.Instance.AddPopToQueue(otherPoppables);
        }

        public Poppable[] GetPoppablesInRange ()
        {
            // if you want some other collision behavior, modify this to match (perhaps reference a collider in the child implementation, could even be the highlightObject)
            var others = Physics.OverlapSphere(transform.position, PopRadius, PoppableLayerMask); // consider OverlapSphereNonAlloc
            var otherPoppables = others.Select(sel => sel.GetComponent<Poppable>())
                .Where(wh => wh != this && wh != null).ToArray();
            return otherPoppables;
        }

        public void BodyPhysics()
        {
            bodyRbody.isKinematic = false;
            //bodyRbody.AddForce(Vector3.up * PostPopImpulseForce, ForceMode.VelocityChange);
            //bodyRbody.AddTorque(Vector3.left * Random.Range(0f,1f), ForceMode.Impulse);
            var explosionPos = RandomPointOnGround(bodyCollider.bounds);

            bodyRbody.AddExplosionForce(PostPopImpulseForce * Random.Range(0.8f, 1.2f), explosionPos, PopRadius, 0f, ForceMode.Impulse);
            //bodyRbody.AddForceAtPosition(PostPopImpulseForce * Vector3.up, bodyRbody.transform.position, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherPoppable = other.GetComponent<Poppable>();
            if (otherPoppable != null && otherPoppable != this) {
                otherPoppable.Prime(); // set primed for scoring
                Sequence startPop = DOTween.Sequence();
                //startPop.AppendInterval(popDelay);
                //startPop.AppendCallback(() => PopManager.Instance.AddPopToQueue(this));
                // Reporting immediately to ensure all pops known in this frame (to prevent early end state)
                PopManager.Instance.AddPopToQueue(this);
            }
        }

        public void ShowHighlight (bool show) {
            ExplosionHighlight.SetActive(show);
        }

        public static Vector3 RandomPointInBounds(Bounds bounds) {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static Vector3 RandomPointOnBounds (Bounds bounds) {
            var axisToMax = Random.Range(0, 3);
            return new Vector3(
                axisToMax == 0 ? bounds.max.x : Random.Range(bounds.min.x, bounds.max.x),
                axisToMax == 1 ? bounds.max.y : Random.Range(bounds.min.y, bounds.max.y),
                axisToMax == 2 ? bounds.max.z : Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static Vector3 RandomPointOnGround (Bounds bounds) {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0,
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        protected override void OnExplosionComplete () {
            // This hook triggers after the hit objects have been exploded
            //Destroy(gameObject);
            GameController.Instance.Explodables.Remove(this);
            // TODO Schedule cleanup?
        }
    }
}

