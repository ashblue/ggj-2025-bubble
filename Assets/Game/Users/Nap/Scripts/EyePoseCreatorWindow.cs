using System.Linq;
using UnityEngine;
using UnityEditor;

namespace GameJammers.GGJ2025.Emote {
    public class EyePoseCreatorWindow : EditorWindow {
        private string poseName = "";
        private string saveDirectory = "Assets/Game/Resources/EyePoses/";
        private GameObject selectedObject;

        [MenuItem("Tools/GGJ/EyePose Creator")]
        public static void ShowWindow() {
            var window = GetWindow<EyePoseCreatorWindow>("EyePose Creator");
            window.minSize = new Vector2(400, 200);
        }

        private void OnGUI() {
            GUILayout.Label("Create a New EyePose", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Display the currently selected GameObject
            selectedObject = Selection.activeGameObject;
            EditorGUILayout.LabelField("Selected Object", selectedObject ? selectedObject.name : "None");

            // Input fields for Pose Name and Save Directory
            EditorGUI.BeginChangeCheck();
            poseName = EditorGUILayout.DelayedTextField("Pose Name", poseName);
            saveDirectory = EditorGUILayout.DelayedTextField("Save Directory", saveDirectory);

            // If Enter is pressed in DelayedTextField, we trigger creation
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(poseName) && !string.IsNullOrEmpty(saveDirectory)) {
                CreateEyePose();
            }

            EditorGUILayout.Space();

            // Create and Cancel buttons
            GUILayout.BeginHorizontal();
            /*// not using this now as you can press enter in the text field (and will error if you click this instead)
            if (GUILayout.Button("Create", GUILayout.Height(40))) {
                CreateEyePose();
            }*/
            if (GUILayout.Button("Cancel", GUILayout.Height(40))) {
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void CreateEyePose() {
            // Validation
            if (selectedObject == null) {
                EditorUtility.DisplayDialog("Error", "Please select a GameObject in the scene.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(poseName)) {
                EditorUtility.DisplayDialog("Error", "Please enter a pose name.", "OK");
                return;
            }

            if (!AssetDatabase.IsValidFolder(saveDirectory)) {
                EditorUtility.DisplayDialog("Error", $"Invalid save directory: {saveDirectory}", "OK");
                return;
            }

            // Find the eyes in the selected object's children
            string leftEyeSuffix = "Eye_Left";
            string rightEyeSuffix = "Eye_Right";
            var children = selectedObject.GetComponentsInChildren<Transform>();
            Transform leftEye = children.FirstOrDefault(wh => wh.name.EndsWith(leftEyeSuffix));
            Transform rightEye = children.FirstOrDefault(wh => wh.name.EndsWith(rightEyeSuffix));

            if (leftEye == null || rightEye == null) {
                EditorUtility.DisplayDialog("Error", $"Could not find '{leftEyeSuffix}' or '{rightEyeSuffix}' as a suffix in the selected object's children: \"{selectedObject.name}\".", "OK");
                return;
            }

            // Create the EyePose
            EyePose newPose = ScriptableObject.CreateInstance<EyePose>();
            newPose.Name = poseName;

            newPose.left = new TransformInfo(
                leftEye.localPosition,
                leftEye.localScale,
                leftEye.localRotation
            );

            newPose.right = new TransformInfo(
                rightEye.localPosition,
                rightEye.localScale,
                rightEye.localRotation
            );

            // Save the new EyePose as an asset
            string savePath = $"{saveDirectory}/{poseName}.asset";
            AssetDatabase.CreateAsset(newPose, savePath);

            // Add the EyePose to the registry
            AddPoseToRegistry(newPose);

            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", $"EyePose '{poseName}' saved to {savePath}", "OK");

            // Close the window after successful creation
            Close();
        }

        private void AddPoseToRegistry(EyePose newPose) {
            // Look for an existing EyePoseRegistry in the save directory
            string registryPath = $"{saveDirectory}/EyePoseRegistry.asset";
            EyePoseRegistry registry = AssetDatabase.LoadAssetAtPath<EyePoseRegistry>(registryPath);

            if (registry == null) {
                // Create a new registry if none exists
                registry = ScriptableObject.CreateInstance<EyePoseRegistry>();
                AssetDatabase.CreateAsset(registry, registryPath);
            }

            // Avoid duplicates and add the new pose
            if (!registry.eyePoses.Contains(newPose)) {
                registry.eyePoses.Add(newPose);
                EditorUtility.SetDirty(registry);
            }
        }
    }
}
