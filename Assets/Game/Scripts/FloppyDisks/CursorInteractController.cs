using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class CursorInteractController : MonoBehaviour {
        static CursorInteractController _instance;
        public static CursorInteractController Instance => _instance;

        [SerializeField]
        Camera _roomCamera;

        [Tooltip("Lock all cursors to this absolute Z position. Which affects how close or far the disk cursor is from the camera")]
        [SerializeField]
        float _cursorWorldZLock = 0.3f;

        enum State {
            HandEmpty,
            HoldingDiskRoom,
            HoldingDiskComputer,
        }

        readonly List<FloppyDisk> _disks = new();
        State _state = State.HandEmpty;
        GameObject _roomDisk;

        void Awake () {
            if (_instance) {
                Debug.LogWarning("Multiple FloppyDiskControllers detected. Destroying this one.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        void OnDestroy () {
            if (_instance) _instance = null;
        }

        public void Add (FloppyDisk disk) {
            _disks.Add(disk);
            Bind(disk);
        }

        void Bind (FloppyDisk disk) {
            disk.EventDiskClick.AddListener(OnClickDisk);
        }

        void OnClickDisk (FloppyDisk disk) {
            if (_state != State.HandEmpty) return;

            // Enter follow cursor room state
            StartCoroutine(HoldingDiskRoomLoop(disk));
        }

        IEnumerator HoldingDiskRoomLoop (FloppyDisk disk) {
            _roomDisk = Instantiate(disk.RoomPrefab);
            _state = State.HoldingDiskRoom;

            while (_state == State.HoldingDiskRoom) {
                var mousePos = Mouse.current.position.ReadValue();
                var worldPos = _roomCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _cursorWorldZLock));
                _roomDisk.transform.position = worldPos;

                if (Mouse.current.rightButton.wasPressedThisFrame) {
                    Destroy(_roomDisk);
                    _state = State.HandEmpty;
                    yield break;
                }

                // @TODO Detect we are over the computer screen
                // Move to HoldingDiskComputerLoop state
                // Remove the room disk, and create the computer disk in world

                yield return null;
            }
        }
    }
}
