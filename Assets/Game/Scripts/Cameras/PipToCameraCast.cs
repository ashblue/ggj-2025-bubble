using System.Collections;
using GameJammers.GGJ2025.FloppyDisks;
using GameJammers.GGJ2025.GodMode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace GameJammers.GGJ2025.Cameras {
    public class PipToCameraCast : MonoBehaviour {
        InputAction _clickAction;
        InputAction _pointAction;

        [Tooltip("The camera that is looking at the rendering texture")]
        [SerializeField]
        Camera _cameraMain;

        [Tooltip("The camera our texture is rendering from. Leave empty to auto populate with the first camera found on the layer")]
        [SerializeField]
        Camera _cameraPip;

        public bool IsMouseHover { get; private set; }
        public RaycastHit LastPipRay { get; private set; }
        public bool HasPipWorldPosition { get; private set; }

        void Start () {
            StartCoroutine(InitLoop());
        }

        IEnumerator InitLoop () {
            // We don't have a pip camera, so we'll have to find one async before fully activating
            while (!_cameraPip) {
                SyncCamera();
                yield return null;
            }

            Bind();
        }

        void SyncCamera () {
            var cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var cam in cameras) {
                // Check if the camera has an overlapping layer with the pip camera layers
                var pipLayer = GameSettings.Current.LevelLayer;
                if (pipLayer != (pipLayer | (1 << cam.gameObject.layer))) continue;

                // Make sure it is a base camera (don't accidentally load the UI overlay camera)
                if (cam.GetUniversalAdditionalCameraData().renderType != CameraRenderType.Base) continue;

                _cameraPip = cam;
                break;
            }
        }

        void Bind () {
            _clickAction = InputSystem.actions.FindAction("Click");
            _clickAction.performed += OnClick;

            _pointAction = InputSystem.actions.FindAction("Point");
            _pointAction.performed += OnHover;
        }

        void OnDestroy () {
            _clickAction.performed -= OnClick;
            _pointAction.performed -= OnHover;
        }

        void OnClick (InputAction.CallbackContext ctx) {
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            var (target, _) = GetObjectFromPip(Input.mousePosition);
            target?.PipInteract();
        }

        void OnHover (InputAction.CallbackContext ctx) {
            var (target, screenHover) = GetObjectFromPip(Input.mousePosition);
            IsMouseHover = screenHover;
            target?.PipHover();
        }

        (IInteractableObject target, bool screenHover) GetObjectFromPip (Vector3 mousePosition) {
            HasPipWorldPosition = false;

            // Check if the camera has busted due to scene reload and try to re-sync
            if (!_cameraPip) {
                SyncCamera();
                return (null, false);
            }

            // Confirm the mouse is over this object
            var ray = _cameraMain.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, GameSettings.Current.RoomLayer)) return (null, false);
            if (hit.collider.gameObject != gameObject) return (null, false);

            // Get the exact position on the texture
            var textureCoord = hit.textureCoord;
            var viewportPoint = new Vector3(textureCoord.x, textureCoord.y, 0);
            var renderRay = _cameraPip.ViewportPointToRay(viewportPoint);

            // Cast a ray from the camera to the pip world for physical surfaces
            var pipLayer = GameSettings.Current.LevelLayer;
            if (!Physics.Raycast(renderRay, out var renderHit, Mathf.Infinity, pipLayer, QueryTriggerInteraction.Ignore)) {
                return (null, true);
            }

            // Log we've hit "something" in the pip world that can be collided with
            HasPipWorldPosition = true;
            LastPipRay = renderHit;

            return (renderHit.collider.gameObject.GetComponent<IInteractableObject>(), true);
        }
    }
}
