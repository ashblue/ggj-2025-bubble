namespace GameJammers.GGJ2025.Bootstraps {
    public class RamController {
        public int Max { get; private set; }
        public int Current { get; private set; }

        public void Reset () {
            Max = 0;
            Current = 0;
        }
    }
}
