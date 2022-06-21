using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


namespace HeadsUpDisplay
{
    public class PauseMenu : NetworkBehaviour
    {
        private bool InInventory = false;
        private bool InPauseMenu = false;
        public GameObject pauseMenuUI;
        public GameObject optionsMenuUI;
        public GameObject InventoryMenuUI;
        [HideInInspector] public bool SinglePlayer = false;

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

                if (Input.GetButtonDown(Finals.OPEN_MENU))
                {
                    Pause();
                }
                if (Input.GetButtonDown(Finals.INVENTORY))
                {
                    Inventory();
                }
            }
        }

        private void Inventory()
        {
            InInventory = !InInventory;

            this.InventoryMenuUI.SetActive(InInventory);
        }

        private void Pause()
        {
            this.optionsMenuUI.SetActive(false);

            InPauseMenu = !InPauseMenu;
            this.pauseMenuUI.SetActive(InPauseMenu);
            SetGamePause(InPauseMenu);
        }

        private void SetGamePause(bool setPause)
        {
            if (SinglePlayer)
            {
                Time.timeScale = setPause ? 0f : 1f;
            }

            // Cursor.lockState = setPause ? CursorLockMode.None : CursorLockMode.Locked;
        }

        // used with UI button
        public void Options()
        {
            if (hasAuthority)
            {
                this.pauseMenuUI.SetActive(false);
                this.optionsMenuUI.SetActive(true);
            }
        }

        // used with UI button
        public void BackFromOptions()
        {
            if (hasAuthority)
            {
                this.pauseMenuUI.SetActive(true);
                this.optionsMenuUI.SetActive(false);
            }
        }

        // used with UI button
        public void QuitGame()
        {
            if (hasAuthority)
            {
                InPauseMenu = false;
                SetGamePause(InPauseMenu);

                PlayerObjectController playerObjectController = GetComponentInParent<PlayerObjectController>();
                playerObjectController.LeaveLobby();
            }
        }
    }
}