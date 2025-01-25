using UnityEngine;
using UnityEngine.Serialization;

namespace GameJammers.GGJ2025.Emote {
    [System.Serializable]
    public class TransformInfo {
        [SerializeField] private Vector3 localPosition;
        [SerializeField] private Vector3 localScale;
        [SerializeField] private Quaternion localRotation;

        public Vector3 LocalPosition => localPosition;
        public Vector3 LocalScale => localScale;
        public Quaternion LocalRotation => localRotation;

        public TransformInfo(Vector3 localPosition, Vector3 localScale, Quaternion localRotation) {
            this.localPosition = localPosition;
            this.localScale = localScale;
            this.localRotation = localRotation;
        }
    }

    [CreateAssetMenu(fileName = "EyePose", menuName = "GGJ/Emotes/EyePose", order = 1)]
    public class EyePose : ScriptableObject {
        public string Name;
        [FormerlySerializedAs("LeftEye")] public TransformInfo left;
        [FormerlySerializedAs("RightEye")] public TransformInfo right;
    }

}
