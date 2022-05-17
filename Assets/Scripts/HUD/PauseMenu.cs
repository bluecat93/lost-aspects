using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


namespace HeadsUpDisplay
{
    public class PauseMenu : NetworkBehaviour
    {

        public static bool isGamePaused = false;
        public GameObject pauseMenuUI;
        public GameObject optionsMenuUI;

        string quitButton = "MainMenu";

        // Update is called once per frame
        void Start()
        {
            // TODO only active that it in single player
            // Time.timeScale = 1f;
        }
        void Update()
        {
            if (hasAuthority)
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

        public void QuitGame()
        {
            if (hasAuthority)
            {
                isGamePaused = false;

                // TODO only active that in single player
                Time.timeScale = 1f;

                SceneManager.LoadScene(quitButton);
            }
        }
    }
}