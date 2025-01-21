using System;
using UnityEngine;

namespace GameJammers.GGJ2025.Cameras {
    [Serializable]
    public class CameraBounds {
        [SerializeField]
        Vector2 _position;
        
        [SerializeField]
        Vector2 _size;

        public float Width => _size.x;
        public float Height => _size.y;
        public Vector2 Position => _position;
        
        public float XMin => _position.x - _size.x / 2;
        public float XMax => _position.x + _size.x / 2;
        public float YMin => _position.y - _size.y / 2;
        public float YMax => _position.y + _size.y / 2;
        
        public CameraBounds() {}
        
        public CameraBounds(float x, float y, float width, float height) {
            _size = new Vector2(width, height);
        }
    }
}