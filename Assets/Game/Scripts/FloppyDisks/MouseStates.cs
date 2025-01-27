
using GameJammers.GGJ2025.Cameras;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJammers.GGJ2025.FloppyDisks {
    [RequireComponent(typeof(CursorInteractController))]
    public class MouseStates : MonoBehaviour {
        [Header("Prefabs")]

        [Tooltip("The prefab used to represent the mouse's default model")]
        [SerializeField]
        GameObject _defaultMouse;

        [Tooltip("The prefab used to represent the mouse's model for when it can click")]
        [SerializeField]
        GameObject _hoverMouse;

        [Tooltip("The prefab used to represent the mouse's model for when it can not place an object")]
        [SerializeField]
        GameObject _errorMouse;

        [SerializeField]
        Camera _roomCamera;

        [Tooltip("Lock all cursors to this absolute Z position. Which affects how close or far the cursor is from the camera")]
        [SerializeField]
        float _cursorWorldZLock = 0.2f;

        CursorInteractController _cursor;

        GameObject defaultMouseObject;
        GameObject hoverMouseObject;
        GameObject errorMouseObject;

        GameObject ActiveObject {
            get => _activeObject;
            set {
                if (_activeObject != null) {
                    _activeObject.SetActive(false);
                }
                _activeObject = value;
                if (value != null) {
                    _activeObject.SetActive(true);
                }
            }
        }
        GameObject _activeObject;


        void Awake () {
            _cursor = GetComponent<CursorInteractController>();
            _cursor.StateChanged += OnStateChanged;
            ActiveObject = defaultMouseObject = Instantiate(_defaultMouse);
            hoverMouseObject = Instantiate(_hoverMouse);
            errorMouseObject = Instantiate(_errorMouse);
            hoverMouseObject.SetActive(false);
            errorMouseObject.SetActive(false);
        }

        void OnDestroy() {
            _cursor.StateChanged -= OnStateChanged;
        }

        public void OnStateChanged (CursorInteractController.State newState) {
            ActiveObject = newState == CursorInteractController.State.HoldingDiskRoom ?
                hoverMouseObject : defaultMouseObject;
        }

        void Update() {
            if (ActiveObject == null) { return; }
            var mousePos = Mouse.current.position.ReadValue();
            ActiveObject.transform.position = _roomCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _cursorWorldZLock));

            if (ActiveObject == defaultMouseObject) { return; }
            ActiveObject = _cursor.CanPlace ? hoverMouseObject : errorMouseObject;
        }
    }
}
