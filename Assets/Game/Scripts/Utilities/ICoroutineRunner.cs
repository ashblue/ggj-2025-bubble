using UnityEngine;

namespace GameJammers.GGJ2025.Utilities {
    /// <summary>
    /// A utility interface so coroutine runners can be passed down into other objects
    /// </summary>
    public interface ICoroutineRunner {
        Coroutine StartCoroutine(System.Collections.IEnumerator routine);
        void StopCoroutine (Coroutine loop);
    }
}
