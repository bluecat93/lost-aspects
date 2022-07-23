using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;

namespace HeadsUpDisplay
{
    public class InGameMenu : NetworkBehaviour
    {
        private bool InInventory = false;
        private bool InCrafting = false;
        private bool InPauseMenu = false;
        public GameObject pauseMenuUI;
        public GameObject optionsMenuUI;
        public GameObject InventoryMenuUI;
        public GameObject CraftingMenuUI;
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
                if (Input.GetButtonDown(Finals.CRAFTING))
                {
                    Crafting();
                }
            }
        }

        private void Crafting()
        {
            InCrafting = !InCrafting;
            SetCursorLock(true);
            if (InCrafting)
            {
                GetComponent<Crafting.KnownRecipes>().UpdateRecipes();
            }
            this.CraftingMenuUI.SetActive(InCrafting);
        }

        private void Inventory()
        {
            InInventory = !InInventory;
            SetCursorLock(true);
            this.InventoryMenuUI.SetActive(InInventory);
        }

        private void Pause()
        {
            this.optionsMenuUI.SetActive(false);

            InPauseMenu = !InPauseMenu;
            this.pauseMenuUI.SetActive(InPauseMenu);
            SetGamePause(InPauseMenu);
            SetCursorLock(true);
        }

        private void SetGamePause(bool setPause)
        {
            if (SinglePlayer)
            {
                Time.timeScale = setPause ? 0f : 1f;
            }
        }

        private void SetCursorLock(bool isLockingCursor)
        {
            if (CurserLocker.isCursorLocked)
            {
                if (isLockingCursor && !InInventory && !InCrafting && !InPauseMenu)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
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
                SetCursorLock(InPauseMenu);

                PlayerObjectController playerObjectController = GetComponentInParent<PlayerObjectController>();
                playerObjectController.LeaveLobby();
            }
        }
    }
}