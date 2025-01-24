using GameJammers.GGJ2025.Bubble;
using UnityEngine;

namespace GameJammers.GGJ2025.Bubble{
    public class DoPop : MonoBehaviour
    {
        Poppable thisPoppable;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            thisPoppable = GetComponent<Poppable>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PopManager.Instance.AddPopToQueue(thisPoppable);
            }
        }
    }
}

