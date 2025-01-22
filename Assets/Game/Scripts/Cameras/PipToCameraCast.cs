using System.Collections;
using GameJammers.GGJ2025.GodMode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJammers.GGJ2025.Cameras {
    public class PipToCameraCast : MonoBehaviour {
        InputAction _clickAction;
        InputAction _pointAction;

        [Tooltip("The camera that is looking at the rendering texture")]
        [SerializeField]
        Camera _cameraMain;

        [Tooltip("Optimize initial clicks by setting a specific layer for the main camera casts")]
        [SerializeField]
        LayerMask _cameraMainLayer;
        
        [Tooltip("The camera our texture is rendering from. Leave empty to auto populate with the first camera found on the layer")]
        [SerializeField]
        Camera _cameraPip;
        
        [Tooltip("Set a specific layer for the pip camera casts for performance and correctness")]
        [SerializeField]
        LayerMask _cameraPipLayer;

        void Start () {
            StartCoroutine(InitLoop());
        }
        
        IEnumerator InitLoop () {
            // We don't have a pip camera, so we'll have to find one async before fully activating
            while (!_cameraPip) {
                var cameras = FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                foreach (var cam in cameras) {
                    // Check if the camera has an overlapping layer with the pip camera layers
                    if (_cameraPipLayer != (_cameraPipLayer | (1 << cam.gameObject.layer))) continue;
                    _cameraPip = cam;
                    break;
                }

                yield return null;
            }
            
            Bind();
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
            var target = GetObjectFromPip(Input.mousePosition);
            target?.PipInteract();
        }
        
        void OnHover (InputAction.CallbackContext ctx) {
            var target = GetObjectFromPip(Input.mousePosition);
            target?.PipHover();
        }

        IInteractableObject GetObjectFromPip (Vector3 mousePosition) {
            // Confirm a click was made to this object
            var ray = _cameraMain.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _cameraMainLayer)) return null;
            if (hit.collider.gameObject != gameObject) return null;
            
            // Get the exact click position on the texture
            var textureCoord = hit.textureCoord;
            var viewportPoint = new Vector3(textureCoord.x, textureCoord.y, 0);
            var renderRay = _cameraPip.ViewportPointToRay(viewportPoint);
            
            // Cast a ray from the camera to the pip world
            if (!Physics.Raycast(renderRay, out var renderHit, Mathf.Infinity, _cameraPipLayer)) return null;
            return renderHit.collider.gameObject.GetComponent<IInteractableObject>();
        }
    }
}