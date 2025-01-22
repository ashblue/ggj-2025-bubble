using UnityEngine;

namespace GameJammers.GGJ2025.GodMode {
    public interface IInteractableObject {
        void PipInteract();
        void PipHover();
    }

    /// <summary>
    /// This base class should be extended by any object that can be interacted with in the PiP view
    /// </summary>
    public abstract class InteractableObjectBase : MonoBehaviour, IInteractableObject {
        public void PipInteract() {
            OnPipInteract();
        }

        /// <summary>
        /// Override this for custom behavior when the object is interacted with
        /// </summary>
        protected virtual void OnPipInteract() {
        }

        public void PipHover() {
            OnPipHover();
        }

        /// <summary>
        /// Override this for custom behavior when the object is hovered over
        /// </summary>
        protected virtual void OnPipHover() {
        }
    }
}