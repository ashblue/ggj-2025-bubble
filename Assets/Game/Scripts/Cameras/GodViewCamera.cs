using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJammers.GGJ2025.Cameras {
    public class GodViewCamera : MonoBehaviour {
        InputAction _moveAction;
        InputAction _rotateAction;

        [Tooltip("An absolute positioned camera boundary that limits camera movement. Position marks the CENTER of the rectangle.")]
        [SerializeField]
        CameraBounds _bounds = new CameraBounds(0, 0, 100, 100);

        [Tooltip("The speed at which the camera moves")]
        [SerializeField]
        float _moveSpeed = 15f;

        [Tooltip("The speed at which the camera rotates")]
        [SerializeField]
        float _rotateSpeed = 15f;

        void Awake () {
            _moveAction = InputSystem.actions.FindAction("Move");
            _rotateAction = InputSystem.actions.FindAction("Rotate");
        }

        void Update () {
            var rotate = _rotateAction.ReadValue<float>();
            transform.Rotate(Vector3.up, rotate * _rotateSpeed * Time.deltaTime, Space.World);

            var move = _moveAction.ReadValue<Vector2>();
            var rotationAngle = transform.rotation.eulerAngles.y;
            var moveVector = Quaternion.Euler(0, rotationAngle, 0) * new Vector3(move.x, 0, move.y);
            var position = transform.position + moveVector * (_moveSpeed * Time.deltaTime);

            // Clamp positions
            position.x = Mathf.Clamp(position.x, _bounds.XMin, _bounds.XMax);
            position.z = Mathf.Clamp(position.z, _bounds.YMin, _bounds.YMax);

            transform.position = position;
        }

        void OnDrawGizmosSelected () {
            Gizmos.color = Color.green;

            // Draw center
            Gizmos.DrawSphere(new Vector3(_bounds.Position.x, 0, _bounds.Position.y), 0.5f);

            // Draw Bounds
            Gizmos.DrawWireCube(
                new Vector3(_bounds.Position.x, 0, _bounds.Position.y),
                new Vector3(_bounds.Width, 0, _bounds.Height));
        }
    }
}
