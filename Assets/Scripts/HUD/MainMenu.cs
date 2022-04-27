using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeadsUpDisplay
{
    public class MainMenu : MonoBehaviour
    {
        public Scene MultiScene;
        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void QuitGame()
        {
            Debug.Log("Game cant quit in-engine");
            Application.Quit();
        }

        public void MultiplayerMenu()
        {
            SceneManager.LoadScene("MultiplayerTestingMenu");
        }
    }
}

