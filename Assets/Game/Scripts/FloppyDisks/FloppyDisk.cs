using ChrisNolet.QuickOutline;
using UnityEngine;
using UnityEngine.Events;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class FloppyDisk : MonoBehaviour {
        [Tooltip("How much RAM this floppy disk adds to the computer")]
        [SerializeField]
        int _ram = 1;

        [Header("Prefabs")]

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

        private Outline _outline;


        public int Ram => _ram;

        void Start () {
            CursorInteractController.Instance.Add(this);
            TryGetComponent(out _outline);
            if (_outline != null) { _outline.enabled = false; }
        }

        void OnMouseDown () {
            EventDiskClick.Invoke(this);
            if (_outline != null) { _outline.OutlineColor = Color.white; }
        }

        void OnMouseEnter () {
            if (_outline == null) { return; }
            _outline.enabled = true;
        }

        void OnMouseExit () {
            if (_outline == null) { return; }
            _outline.enabled = false;
        }
    }
}
