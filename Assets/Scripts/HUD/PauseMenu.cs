using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


namespace HeadsUpDisplay
{
    public class PauseMenu : NetworkBehaviour
    {

        public bool isGamePaused = false;
        public GameObject pauseMenuUI;
        public GameObject optionsMenuUI;

        string quitButton = "MainMenu";

        // Update is called once per frame
        void Start()
        {
            // TODO only active that in single player
            // Time.timeScale = 1f;
        }
        void Update()
        {
            if (hasAuthority)
            {
                // TODO change keycode to an actual name
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
        }
        public void Resume()
        {
            if (hasAuthority)
            {
                this.pauseMenuUI.SetActive(false);
                this.optionsMenuUI.SetActive(false);

                // TODO only active that in single player
                // Time.timeScale = 1f;

                // Debug.Log("time is now:" + Time.deltaTime);
                isGamePaused = false;
                // Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
            }
        }

        public void Pause()
        {
            if (hasAuthority)
            {
                this.optionsMenuUI.SetActive(false);
                this.pauseMenuUI.SetActive(true);

                // TODO only active that in single player
                // Time.timeScale = 0f;

                // Debug.Log("time is now:"+Time.deltaTime);
                isGamePaused = true;
                // Cursor.lockState = CursorLockMode.None;
            }
        }

        public void Options()
        {
            if (hasAuthority)
            {
                this.pauseMenuUI.SetActive(false);
                this.optionsMenuUI.SetActive(true);
            }
        }

        public void BackFromOptions()
        {
            if (hasAuthority)
            {
                this.pauseMenuUI.SetActive(true);
                this.optionsMenuUI.SetActive(false);
            }
        }

        public void QuitGame()
        {
            // TODO quit game should destroy player and reset everything back to when you just run the game for the first time.
            if (hasAuthority)
            {
                Debug.Log("Quit game is on a work in progress.");
                // isGamePaused = false;

                // TODO only active that in single player
                // Time.timeScale = 1f;

                // SceneManager.LoadScene(quitButton);
            }
        }
    }
}