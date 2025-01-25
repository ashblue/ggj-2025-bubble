using UnityEngine;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class LevelController : MonoBehaviour {
        [Tooltip("The maximum amount of RAM the player can have from floppy disks")]
        [SerializeField]
        int _maxRam = 20;

        void Start () {
            var ram = GameController.Instance.Ram;
            ram.Reset();
            ram.SetMax(_maxRam);
        }
    }
}
