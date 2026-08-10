using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01_Scripts._06_General
{
    public class MenuBehaviour : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSaveGame()
        {
            
        }

        public void EndGame()
        {
            Application.Quit();
        }
    }
}