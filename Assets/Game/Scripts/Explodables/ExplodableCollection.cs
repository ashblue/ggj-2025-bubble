using System.Collections.Generic;

namespace GameJammers.GGJ2025.Explodables {
    public class ExplodableCollection {
        readonly List<IExplodable> _items = new();
        readonly List<IExplodable> _itemsExploding = new();
        readonly List<IExplodable> _debris = new();

        public IReadOnlyList<IExplodable> Items => _items;
        public IReadOnlyList<IExplodable> ItemsExploding => _itemsExploding;

        public void Add (IExplodable item) {
            _items.Add(item);
        }

        public void Remove (IExplodable item) {
            _items.Remove(item);
            _debris.Add(item);
        }

        public void Cleanup (IExplodable item) {
            _debris.Remove(item);
        }

        public void AddExploding (IExplodable item) {
            _itemsExploding.Add(item);
        }

        public void RemoveExploding (IExplodable item) {
            _itemsExploding.Remove(item);
        }
    }
}
