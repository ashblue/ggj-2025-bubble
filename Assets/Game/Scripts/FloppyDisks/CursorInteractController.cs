using System.Collections;
using System.Collections.Generic;
using GameJammers.GGJ2025.Cameras;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class CursorInteractController : MonoBehaviour {
        static CursorInteractController _instance;
        public static CursorInteractController Instance => _instance;

        readonly List<FloppyDisk> _disks = new();

        State _state = State.HandEmpty;
        GameObject _roomDisk;
        GameObject _computerPreview;
        Coroutine _loop;

        [SerializeField]
        Camera _roomCamera;

        [Tooltip("Lock all cursors to this absolute Z position. Which affects how close or far the disk cursor is from the camera")]
        [SerializeField]
        float _cursorWorldZLock = 0.3f;

        [SerializeField]
        PipToCameraCast _pipToCamera;

        enum State {
            HandEmpty,
            HoldingDiskRoom,
            HoldingDiskComputer,
            Lock,
        }

        void Awake () {
            if (_instance) {
                Debug.LogWarning("Multiple FloppyDiskControllers detected. Destroying this one.");
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        void OnDestroy () {
            if (_instance == this) _instance = null;
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
            _loop = StartCoroutine(HoldingDiskLoop(disk));
        }

        IEnumerator HoldingDiskLoop (FloppyDisk disk) {
            _state = State.HoldingDiskRoom;

            _roomDisk = Instantiate(disk.RoomPrefab);
            _computerPreview = Instantiate(disk.ComputerPreviewPrefab);
            _computerPreview.SetActive(false);

            while (_state != State.HandEmpty) {
                var mousePos = Mouse.current.position.ReadValue();
                var worldPos = _roomCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _cursorWorldZLock));
                _roomDisk.transform.position = worldPos;

                // Handle canceling the disk placement
                if (Mouse.current.rightButton.wasPressedThisFrame) {
                    Stop();
                    yield break;
                }

                // @NOTE Not the prettiest things I've written, but it runs fine...
                // Detect we are over the computer screen
                if (_pipToCamera.IsMouseHover) {
                    _roomDisk.SetActive(false);
                    _computerPreview.SetActive(false);

                    // Put the placement prefab in the world
                    if (_pipToCamera.HasPipWorldPosition) {
                        var target = _pipToCamera.LastPipRay.collider.gameObject;

                        // Only target ground so we know it's safe to place the prefab there
                        var groundLayer = GameSettings.Current.DiskPlacementLayer;
                        if (groundLayer == (groundLayer | (1 << target.layer))) {
                            _computerPreview.SetActive(true);
                            var position = _pipToCamera.LastPipRay.point;
                            ShowDiskPreview(position);
                        }
                    }
                } else {
                    _roomDisk.SetActive(true);
                    _computerPreview.SetActive(false);
                }

                if (_roomDisk.activeSelf)
                    MouseStates.Instance.ChangeState(MouseStates.State.Hover);
                else if (_computerPreview.activeSelf) {
                    MouseStates.Instance.ChangeState(MouseStates.State.Hover);
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                        SpawnDisk(disk, _computerPreview.transform.position);
                }
                else
                    MouseStates.Instance.ChangeState(MouseStates.State.Error);

                yield return null;
            }
            MouseStates.Instance.ChangeState(MouseStates.State.Default);
            _loop = null;
        }

        void ShowDiskPreview (Vector3 position) {
            _computerPreview.transform.position = position;
        }

        void SpawnDisk (FloppyDisk disk, Vector3 position) {
            // Force spawned objects into the level transform to prevent leaking between scene loads
            var parent = LevelController.Instance.transform;

            Instantiate(disk.ComputerPrefab, position, Quaternion.identity, parent);
            GameController.Instance.Ram.Add(disk.Ram);
        }

        void Stop () {
            if (_loop != null) StopCoroutine(_loop);
            if (_roomDisk) Destroy(_roomDisk);
            if (_computerPreview) Destroy(_computerPreview);
            MouseStates.Instance.ChangeState(MouseStates.State.Default);
            _state = State.HandEmpty;
            _loop = null;
        }

        public void Lock () {
            Stop();
            _state = State.Lock;
        }

        public void Reset () {
            _state = State.HandEmpty;
        }
    }
}
