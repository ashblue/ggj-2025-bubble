using UnityEngine;

namespace GameJammers.GGJ2025.GodMode
{
    public class InteractableObjectExample : InteractableObjectBase
    {
        protected override void OnPipInteract()
        {
            Debug.Log("Interacted with " + name);
        }

        protected override void OnPipHover()
        {
            Debug.Log("Hovered over " + name);
        }
    }
}