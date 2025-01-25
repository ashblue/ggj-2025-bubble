using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class LevelController : MonoBehaviour {
        public static LevelController Instance { get; private set; }

        [Tooltip("The maximum amount of RAM the player can have from floppy disks")]
        [SerializeField]
        int _maxRam = 20;

        void Start () {
            Instance = this;

            var ram = GameController.Instance.Ram;
            ram.Reset();
            ram.SetMax(_maxRam);
        }

        void OnDestroy () {
            if (Instance == this) Instance = null;
        }
    }
}
