namespace GameJammers.GGJ2025.Explodables {
    public interface IExplodable {
        bool IsPrimer { get; }
        bool IsObjective { get; }

        void Explode ();
    }
}
