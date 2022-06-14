using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


namespace HeadsUpDisplay
{
    public class PauseMenu : NetworkBehaviour
    {

        private bool isGamePaused = false;
        public GameObject pauseMenuUI;
        public GameObject optionsMenuUI;
        public bool SinglePlayer = false;

        // string quitButton = "MainMenu";

        // Update is called once per frame
        void Start()
        {
            if (SinglePlayer)
            {
                Time.timeScale = 1f;
            }
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

                if (SinglePlayer)
                {
                    Time.timeScale = 1f;
                }

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
                if (SinglePlayer)
                {
                    Time.timeScale = 0f;
                }
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
            if (hasAuthority)
            {
                PlayerObjectController playerObjectController = GetComponentInParent<PlayerObjectController>();
                playerObjectController.LeaveLobby();

                // isGamePaused = false;

                // TODO only active that in single player
                // Time.timeScale = 1f;
            }
        }
    }
}