using UnityEngine;
using UnityEngine.Events;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class FloppyDisk : MonoBehaviour {
        [Tooltip("The prefab used to represent holding the floppy disk in the room")]
        [SerializeField]
        GameObject _roomPrefab;

        [Tooltip("The prefab that represents placing the floppy disk on the computer screen")]
        [SerializeField]
        GameObject _computerPreviewPrefab;

        [Tooltip("The prefab that will be spawned when the floppy disk is dropped inside the computer")]
        [SerializeField]
        GameObject _computerPrefab;

        public UnityEvent<FloppyDisk> EventDiskClick { get; } = new();

        public GameObject RoomPrefab => _roomPrefab;
        public GameObject ComputerPreviewPrefab => _computerPreviewPrefab;

        [Tooltip("Note all spawned world items are manually rotated to face the camera, hopefully this doesn't cause any issues")]
        public GameObject ComputerPrefab => _computerPrefab;

        void Start () {
            CursorInteractController.Instance.Add(this);
        }

        void OnMouseDown () {
            EventDiskClick.Invoke(this);
        }
    }
}
