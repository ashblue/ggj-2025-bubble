using UnityEngine.Events;

namespace GameJammers.GGJ2025.FloppyDisks {
    public class RamController {
        public int Max { get; private set; }
        public int Current { get; private set; }

        public UnityEvent<int, int> EventRamChanged { get; } = new();

        public void Reset () {
            Max = 0;
            Current = 0;

            EventRamChanged.Invoke(Current, Max);
        }

        public void Add (int ram) {
            Current += ram;

            EventRamChanged.Invoke(Current, Max);
        }

        public void SetMax (int maxRam) {
            Max = maxRam;

            EventRamChanged.Invoke(Current, Max);
        }
    }
}
