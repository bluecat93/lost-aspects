using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HeadsUpDisplay
{
    public class PauseMenu : MonoBehaviour
    {

        public static bool isGamePaused = false;
        public GameObject pauseMenuUI;
        public GameObject optionsMenuUI;

        string quitButton = "MainMenu";

        // Update is called once per frame
        void Start()
        {
            Time.timeScale = 1f;
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isGamePaused)
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
            this.pauseMenuUI.SetActive(false);
            this.optionsMenuUI.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("time is now:" + Time.deltaTime);
            isGamePaused = false;
            Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
        }

        public void Pause()
        {
            this.optionsMenuUI.SetActive(false);
            this.pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            // Debug.Log("time is now:"+Time.deltaTime);
            isGamePaused = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Options()
        {
            this.pauseMenuUI.SetActive(false);
            this.optionsMenuUI.SetActive(true);
        }

        public void QuitGame()
        {
            isGamePaused = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(quitButton);
        }
    }
}