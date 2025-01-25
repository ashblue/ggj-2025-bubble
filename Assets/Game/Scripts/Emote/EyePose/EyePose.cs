using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using System.Linq;

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

    [CustomEditor(typeof(EyePose), true)]
    public class EyePoseEditor : Editor {
        public override void OnInspectorGUI() {
            // Draw the default inspector
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            // Button to apply the current settings to the selected object
            if (GUILayout.Button("Apply EyePose To Selected")) {
                ApplyEyePoseToSelected();
            }

            EditorGUILayout.Space();

            // Button to update the current target based on the selected object
            if (GUILayout.Button("Update EyePose From Selected")) {
                UpdateEyePoseFromSelected();
            }
        }

        private (GameObject selectedObject, Transform leftEye, Transform rightEye) CommonChecksAndGetEyes () {
            // Ensure a GameObject is selected
            if (Selection.activeGameObject == null) {
                Debug.LogWarning("Please select an applicable GameObject in the scene.");
                return (null, null, null);
            }

            GameObject selectedObject = Selection.activeGameObject;

            // Find the eyes in the selected object's children
            string leftEyeSuffix = "Eye_Left";
            string rightEyeSuffix = "Eye_Right";
            var children = selectedObject.GetComponentsInChildren<Transform>();
            Transform leftEye = children.FirstOrDefault(wh => wh.name.EndsWith(leftEyeSuffix));
            Transform rightEye = children.FirstOrDefault(wh => wh.name.EndsWith(rightEyeSuffix));

            if (leftEye == null || rightEye == null) {
                EditorUtility.DisplayDialog("Error", $"Could not find '{leftEyeSuffix}' or '{rightEyeSuffix}' as a suffix in the selected object's children: \"{selectedObject.name}\".", "OK");
                return (null, null, null);
            }

            return (selectedObject, leftEye, rightEye);
        }

        private void ApplyEyePoseToSelected() {
            (GameObject selectedObject, Transform leftEye, Transform rightEye) = CommonChecksAndGetEyes();

            if (selectedObject == null) {
                return;
            }

            // apply the pose
            EyePose thisEyePose = (EyePose)target;

            leftEye.localPosition = thisEyePose.left.LocalPosition;
            leftEye.localRotation = thisEyePose.left.LocalRotation;
            leftEye.localScale = thisEyePose.left.LocalScale;

            rightEye.localPosition = thisEyePose.right.LocalPosition;
            rightEye.localRotation = thisEyePose.right.LocalRotation;
            rightEye.localScale = thisEyePose.right.LocalScale;

            Debug.Log($"EyePose '{thisEyePose.name}' applied to {selectedObject.name}");
        }

        private void UpdateEyePoseFromSelected() {
            (GameObject selectedObject, Transform leftEye, Transform rightEye) = CommonChecksAndGetEyes();

            if (selectedObject == null) {
                return;
            }

            // Modify the target

            SerializedObject thisTarget = new SerializedObject(target);

            SerializedProperty thisLeftEye = thisTarget.FindProperty("LeftEye");
            SerializedProperty leftEye_localPosition = thisLeftEye.FindPropertyRelative("localPosition");
            SerializedProperty leftEye_localScale = thisLeftEye.FindPropertyRelative("localScale");
            SerializedProperty leftEye_localRotation = thisLeftEye.FindPropertyRelative("localRotation");

            SerializedProperty thisRightEye = thisTarget.FindProperty("RightEye");
            SerializedProperty rightEye_localPosition = thisRightEye.FindPropertyRelative("localPosition");
            SerializedProperty rightEye_localScale = thisRightEye.FindPropertyRelative("localScale");
            SerializedProperty rightEye_localRotation = thisRightEye.FindPropertyRelative("localRotation");

            leftEye_localPosition.vector3Value = leftEye.localPosition;
            leftEye_localScale.vector3Value = leftEye.localScale;
            leftEye_localRotation.quaternionValue = leftEye.localRotation;

            rightEye_localPosition.vector3Value = rightEye.localPosition;
            rightEye_localScale.vector3Value = rightEye.localScale;
            rightEye_localRotation.quaternionValue = rightEye.localRotation;


            thisTarget.ApplyModifiedProperties();

            Debug.Log($"Updated EyePose from {selectedObject.name}");
        }
    }
}
