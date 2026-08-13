using _01_Scripts._05_InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01_Scripts._06_General
{
    public class PauseMenuBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        private bool _paused;

        private void Awake()
        {
            _paused = false;
            pauseMenu.SetActive(false);
            
        }
        
        private void Update()
        {
            if (InputManager.Input.Camera.Pause.WasPressedThisFrame())
            {
                if (_paused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            _paused = false;
        }

        private void Pause()
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            _paused = true;
        }

        public void Restart()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            _paused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SaveGame()
        {
            
        }

        public void BackToMenu(string sceneName)
        {
            Resume();
            SceneManager.LoadScene(sceneName);
        }
        
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
