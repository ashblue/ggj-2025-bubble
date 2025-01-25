namespace GameJammers.GGJ2025.Explodables {
    public interface IExplodable {
        bool AutoExplode { get; }
        bool IsObjective { get; }
        bool IsPrimed { get; }

        void Explode ();
    }
}
