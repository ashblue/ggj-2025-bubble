using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GameJammers.GGJ2025.Bubble
{
    [RequireComponent(typeof(SphereCollider))]
    public class Poppable : MonoBehaviour
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

        private Sequence popSequence;
        public bool canPop = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            bodyRbody = Body.GetComponent<Rigidbody>();
            bodyCollider = Body.GetComponent<BoxCollider>();
            BubbleTopMat = BubbleTop.GetComponent<Renderer>().material;
            BubbleBottomMat = BubbleBottom.GetComponent<Renderer>().material;
            BubbleArmsMat = BubbleArms.GetComponent<Renderer>().material;
            DistortionAreaMat = DistortionArea.GetComponent<Renderer>().material;

            popSphereCollider = GetComponent<SphereCollider>();

            //popSequence = DOTween.Sequence();
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
            popSequence.InsertCallback(disableTime, BodyPhysics);
            // add initial delay (based on bubble vfx)
            popSequence.PrependInterval(0.3f);

            // after pop, pop up the body and apply gravity

            //popSequence.AppendCallback(BodyPhysics);

        }

        public void PopOthers() {
            var otherPoppables = GetPoppablesInRange();
            PopManager.Instance.AddPopToQueue(otherPoppables);
        }

        public Poppable[] GetPoppablesInRange ()
        {
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
            if (otherPoppable != null && otherPoppable != this)
            {
                Sequence startPop = DOTween.Sequence();
                startPop.AppendInterval(popDelay);
                startPop.AppendCallback(() => PopManager.Instance.AddPopToQueue(this));
            }
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
    }
}

