using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameJammers.GGJ2025.Bubble
{
    [RequireComponent(typeof(SphereCollider))]
    public class Poppable : MonoBehaviour
    {
        [SerializeField] private GameObject BubbleTop;
        [SerializeField] private GameObject BubbleBottom;
        [SerializeField] private GameObject BubbleLeftArm;
        [SerializeField] private GameObject BubbleRightArm;
        [SerializeField] private GameObject Body;
        [SerializeField] private GameObject DistortionArea;

        public float PostPopImpulseForce = 5f;
        public float PopRadius = 5f;
        public float popDelay = 0.05f;
        public LayerMask PoppableLayerMask;
        
        
        private Material BubbleTopMat;
        private Material BubbleBottomMat;
        private Material BubbleLeftArmMat;
        private Material BubbleRightArmMat;
        private Material DistortionAreaMat;

        private Rigidbody bodyRbody;

        private SphereCollider popSphereCollider;

        private Sequence popSequence;
        public bool canPop = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            bodyRbody = Body.GetComponent<Rigidbody>();
            BubbleTopMat = BubbleTop.GetComponent<Renderer>().material;
            BubbleBottomMat = BubbleBottom.GetComponent<Renderer>().material;
            BubbleLeftArmMat = BubbleLeftArm.GetComponent<Renderer>().material;
            BubbleRightArmMat = BubbleRightArm.GetComponent<Renderer>().material;
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
            popSequence.Insert(popStart, BubbleLeftArmMat.DOFloat(1, "_PopStep", popDuration));
            popSequence.Insert(popStart, BubbleRightArmMat.DOFloat(1, "_PopStep", popDuration));

            float disableTime = popStart + popDuration * 0.5f;
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleTop.gameObject.SetActive(false));
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleBottom.gameObject.SetActive(false));
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleLeftArm.gameObject.SetActive(false));
            popSequence.InsertCallback(popStart + popDuration, () =>  BubbleRightArm.gameObject.SetActive(false));
            
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

        public void PopOthers()
        {
            var others = Physics.OverlapSphere(transform.position, PopRadius, PoppableLayerMask); // consider OverlapSphereNonAlloc
            var otherPoppables = others.Select(sel => sel.GetComponent<Poppable>())
                .Where(wh => wh != this && wh != null);
            PopManager.Instance.AddPopToQueue(otherPoppables.ToArray());
        }

        public void BodyPhysics()
        {
            bodyRbody.isKinematic = false;
            //bodyRbody.AddForce(Vector3.up * PostPopImpulseForce, ForceMode.VelocityChange);
            //bodyRbody.AddTorque(Vector3.left * Random.Range(0f,1f), ForceMode.Impulse);
            bodyRbody.AddExplosionForce(PostPopImpulseForce * Random.Range(0.8f, 1.2f), transform.position, PopRadius, 1, ForceMode.Impulse);
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
    }
}

