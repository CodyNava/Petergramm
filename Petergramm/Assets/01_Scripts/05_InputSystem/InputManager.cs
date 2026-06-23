using UnityEngine;

namespace _01_Scripts._05_InputSystem
{
    public class InputManager : MonoBehaviour
    {
        public static MainInput Input { get; private set; }

        private void Awake()
        {
            if (Input == null)
            {
                Input ??= new MainInput();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            Input?.Enable();
        }

        private void OnDisable()
        {
            Input?.Disable();
        }

        private void OnDestroy()
        {
            Input?.Dispose();
            Input = null;
        }
    }
}