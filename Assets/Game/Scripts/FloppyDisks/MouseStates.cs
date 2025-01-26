
using GameJammers.GGJ2025.Cameras;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class MouseStates : MonoBehaviour {
        static MouseStates _instance;
        public static MouseStates Instance => _instance;

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

        State _state = State.Default;

        GameObject defaultMouseObject;
        GameObject hoverMouseObject;
        GameObject errorMouseObject;

        public enum State {
            Default,
            Hover,
            Error,
        }

        void Awake () {
            if (_instance) {
                Debug.LogWarning("Multiple MouseStates detected. Destroying this one.");
                Destroy(gameObject);
                return;
            }
            _instance = this;
            defaultMouseObject = Instantiate(_defaultMouse);
            hoverMouseObject = Instantiate(_hoverMouse);
            errorMouseObject = Instantiate(_errorMouse);
            hoverMouseObject.SetActive(false);
            errorMouseObject.SetActive(false);
        }

        public void ChangeState(State newState) {
            if (_state == newState) return;
            switch (_state) {
                case State.Default:
                    defaultMouseObject.SetActive(false);
                    break;
                case State.Hover:
                    hoverMouseObject.SetActive(false);
                    break;
                case State.Error:
                    errorMouseObject.SetActive(false);
                    break;
            }
            switch (_state = newState) {
                case State.Default:
                    defaultMouseObject.SetActive(true);
                    break;
                case State.Hover:
                    hoverMouseObject.SetActive(true);
                    break;
                case State.Error:
                    errorMouseObject.SetActive(true);
                    break;
            }
        }

        void Update() {
            var mousePos = Mouse.current.position.ReadValue();
            var worldPos = _roomCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _cursorWorldZLock));
            switch (_state) {
                case State.Default:
                    defaultMouseObject.transform.position = worldPos;
                    defaultMouseObject.transform.LookAt(_roomCamera.transform);
                    break;
                case State.Hover:
                    hoverMouseObject.transform.position = worldPos;
                    hoverMouseObject.transform.LookAt(_roomCamera.transform);
                    break;
                case State.Error:
                    errorMouseObject.transform.position = worldPos;
                    errorMouseObject.transform.LookAt(_roomCamera.transform);
                    break;
            }
        }
    }
}
