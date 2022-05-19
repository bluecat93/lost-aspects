using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsUpDisplay;
using Mirror;

namespace Player
{
    /// <summary> 
    /// This should be assigned to the root object of the player.
    /// Help keep track of the players statistics.
    /// </summary>
    /// <param name="maxHealth"> players maximum health </param>
    /// <param name="currentHealth"> players current health </param>
    /// <param name="maxHunger"> players maximum hunger </param>
    /// <param name="currentHunger"> players current hunger </param>
    /// <param name="healthBar"> assign the script of the health bar in the player hud </param>
    /// <param name="hungerBar"> assign the script of the hunger bar in the player hud </param>
    public class Stats : NetworkBehaviour
    {
        #region Variables
        [Header("Health Variables")]
        [Tooltip("Health character starts with")]
        [SerializeField] private int maxHealth = 100; // can see: all clients + can change: all clients and server
        [Tooltip("Health character currently has")]
        [SerializeField] private int currentHealth; // can see: all clients + can change: all clients and server
        private bool isAlive = true; // can see: all clients + can change: client

        [Header("Stamina Variables")]
        [Tooltip("Stamina character starts with")]
        [SerializeField] private int maxStamina = 100; // can see: client + can change: everyone 
        [Tooltip("Stamina character currently has")]
        [SerializeField] private int currentStamina; // can see: client + can change: client + server

        [Header("Stamina Recharge Variables")]
        [Tooltip("When to start regenerating stamina")]
        [SerializeField] private float regenStartTime = 2f; // can see: client + can change: client
        [Tooltip("How much stamina recharges per tick")]
        [SerializeField] private int regenRateAmount = 1; // can see: client + can change: client
        [Tooltip("How often stamina recharges, lower is better. Can't go lower than 0.01")]
        [SerializeField] private float regenRate = 0.1f; // can see: client + can change: everyone
        [Tooltip("How often stamina recharges when hungry")]
        [SerializeField] private float regenWhenHungry = 0.5f;  // can see: client + can change: client

        // Check if currently regenning
        private Coroutine isRegen;
        // Variables to hold new and hold regen rates when hungry or full
        private float regenRateWhenFull = 1f;
        // private float currentRegenRate = 1f;

        [Header("Basic Movement Variables")]
        [Tooltip("The character's base movement speed")]
        [SerializeField] private float movementSpeed = 5.0f; // can see: client + can change: everyone
        [Tooltip("Acceleration and deceleration")]
        [SerializeField] private float speedChangeRate = 10.0f; // can see: client + can change: client

        [Header("Dashing Variables")]
        [Tooltip("The speed multiplier when sprinting")]
        [SerializeField] private float dashModifier = 1.5f; // can see: client + can change: client

        [Header("Jumping Variables")]
        [Tooltip("The higher this field, the higher the jump")]
        [SerializeField] private float jumpHeight = 5f; // can see: client + can change: everyone

        [Header("Dodging Variables")]
        [Tooltip("The speed of the character while dodgeing")]
        [SerializeField] private float dodgeSpeed = 10f; // can see: client + can change: everyone
        [Tooltip("The time (in seconds) it takes for the character to dodge (from start to finish)")]
        [SerializeField] private float dodgeTime = 0.5f; // can see: client + can change: client
        [Tooltip("How much stamina a roll takes")]
        [SerializeField] private int dodgeCost = -50; // can see: client + can change: client

        [Header("Hunger Variables")]
        [Tooltip("Hunger when character is full")]
        [SerializeField] private int maxHunger = 100; // can see: client + can change: client
        [Tooltip("Characters current hunger rating")]
        [SerializeField] private int currentHunger; // can see: client + can change: client
        [Tooltip("Start lowering hunger")]
        [SerializeField] private float hungerStartTime = 5.0f; // can see: client + can change: client
        [Tooltip("How often to lower hunger")]
        [SerializeField] private float hungerRepeatTime = 10.0f; // can see: client + can change: client

        private bool isHungry = false;

        [Header("UI Bars")]
        [Tooltip("Bar that displays character health")]
        [SerializeField] private BarScript healthBar; // can see: client + can change: client
        [Tooltip("Bar that displays character hunger")]
        [SerializeField] private BarScript hungerBar; // can see: client + can change: client
        [Tooltip("Bar that displays character stamina")]
        [SerializeField] private BarScript staminaBar; // can see: client + can change: client

        [Header("Game objects")]
        [Tooltip("Game over screen")]
        [SerializeField] private GameObject gameOverUI; // can see: client + can change: client
        [Tooltip("Camera that follows player")]
        [SerializeField] private GameObject thirdPersonCamera;  // can see: client + can change: client

        [Header("Other variables")]
        [Tooltip("Reduce fall damage")]
        [SerializeField] private int fallDamageReduction = 0; // can see: client + can change: everyone


        private ThirdPersonMovement _thirdPersonMovement;

        private ThirdPersonMovement ThrdPrsonMvmt
        {
            get
            {
                if (this._thirdPersonMovement == null)
                    this._thirdPersonMovement = GetComponentInChildren<ThirdPersonMovement>();

                return this._thirdPersonMovement;
            }
        }

        RespawnScript respawn;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            this.currentHealth = this.maxHealth;
            this.currentHunger = this.maxHunger;
            this.currentStamina = this.maxStamina;

            this.isAlive = true;

            regenRateWhenFull = regenRate;

            if (hasAuthority)
            {
                // UI stuff is only interesting for the active client
                this.healthBar.SetMax(this.maxHealth);
                this.hungerBar.SetMax(this.maxHunger);
                this.staminaBar.SetMax(this.maxStamina);

                // hunger stuff is only interesting for the active client
                InvokeRepeating("GettingHungry", this.hungerStartTime, this.hungerRepeatTime);
                InvokeRepeating("CheckHunger", 0.5f, 0.5f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Button to test death
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     this.currentHealth = 0;
            // }

            // Check death
            if (this.currentHealth <= 0 && hasAuthority)
            {
                CmdKilled(); // sends the death to everyone

                this.gameOverUI.SetActive(true);

                // TODO restore cursor locks
                // Cursor.lockState = CursorLockMode.None;
            }

        }

        public void Respawn()
        {
            if (hasAuthority)
            {
                this.gameOverUI.SetActive(false);
                this.respawn = FindObjectOfType<RespawnScript>();
                this.respawn.RespawnPlayer(this.gameObject);
                Heal(maxHealth);
                Eat();
                this.thirdPersonCamera.SetActive(true);
                this.isAlive = true;
                // Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
            }
        }

        #region Health Functions

        // changes current health to the new value.
        // to change someone else's health, call this function from server.
        public void ChangeHealth(int newValue)
        {
            if (isClient)
            {
                if (hasAuthority)
                    // Client needs to call server so he can change the health for everyone. 
                    CmdHealthChanged(newValue);

            }
            if (isServer)
            {
                changeHealthOnServer(newValue);
            }
        }

        private void changeHealthOnServer(int newValue)
        {
            this.currentHealth = newValue;
            this.currentHealth = currentHealth > maxHealth ? maxHealth : currentHealth;
            this.currentHealth = currentHealth < 0 ? 0 : currentHealth;

            // Server is calling this function for everyone.
            RpcChangeHealthWithAuthority(currentHealth);

            if (this.currentHealth == 0)
            {
                RpcKilled();
            }
        }

        // changes max health to the new value.
        public void ChangeMaxHealth(int newValue)
        {
            if (isClient)
            {
                if (hasAuthority)
                    // client needs to call server so he can change the health for everyone.
                    CmdMaxHealthChanged(newValue);

            }
            if (isServer)
            {
                // server needs to call all cients to change the health for everyone
                RpcMaxHealthChangedWithAuthority(newValue);
            }
        }

        public void TakeDamage(int damage, bool isBlockable = true)
        {
            if (this.currentHealth > 0 && ((isBlockable && !this.ThrdPrsonMvmt.IsRolling) || (!isBlockable)))
            {
                ChangeHealth(this.currentHealth - damage);
            }
        }

        public void TakePercentileDamage(int damage, bool isBlockable = true)
        {
            this.TakeDamage(damage * 100 / this.maxHealth, isBlockable);
        }

        public void Heal(int amount)
        {
            ChangeHealth(this.currentHealth + amount);
        }

        #endregion

        #region Hunger Functions

        private void GettingHungry()
        {
            if (hasAuthority)
            {
                if (this.currentHunger > 0)
                {
                    this.currentHunger--;
                    this.hungerBar.SetCurrent(this.currentHunger);
                }
            }
        }

        private void CheckHunger()
        {
            if (hasAuthority)
            {
                if (this.currentHunger == 0) // how we know if character is hungry
                {
                    this.isHungry = true;
                    regenRate = regenWhenHungry;
                }
                else
                {
                    this.isHungry = false;
                    regenRate = regenRateWhenFull;
                }
            }
        }

        public void Eat()
        {
            if (hasAuthority)
            {
                this.currentHunger = this.maxHunger;
                this.hungerBar.SetCurrent(this.maxHunger);
            }
        }

        #endregion

        #region Stamina Functions

        public void RestoreStamina(int amount)
        {

            if (isServer)
            {
                RpcRestoreStamina(amount);
            }
            else if (isClient)
            {
                // client can't change this stat unless it has authority.
                if (hasAuthority)
                {
                    RestoreStaminaWithAuthority(amount);
                }

            }
        }

        private void RestoreStaminaWithAuthority(int amount)
        {
            if (!hasAuthority)
                return;

            this.currentStamina += amount;
            this.currentStamina = this.currentStamina > this.maxStamina ? this.maxStamina : this.currentStamina;
            this.currentStamina = this.currentStamina < 0 ? 0 : this.currentStamina;

            this.staminaBar.SetCurrent(this.currentStamina);
            if (this.currentStamina < this.maxStamina)
            {
                if (this.isRegen != null)
                    StopCoroutine(this.isRegen);
                this.isRegen = StartCoroutine(RegenStamina());
            }
        }

        public void FasterRegenRate(float amount)
        {
            if (isClient)
            {
                if (hasAuthority)
                {
                    FasterRegenRateWithAuthority(amount);
                }
                else
                {
                    Debug.LogWarning("ran FasterRegenRate from client without authority. call this function from server instead.");
                }
            }
            else if (isServer)
            {
                RpcFasterRegenRate(amount);
            }
        }

        private void FasterRegenRateWithAuthority(float amount)
        {
            if (!hasAuthority)
                return;
            regenRate -= amount;
            regenRate = regenRate < 0.01f ? 0.01f : regenRate;
        }
        private IEnumerator RegenStamina()
        {
            // Debug.Log("regenning stamina");
            // Start regenerating stamina after a few seconds
            yield return new WaitForSeconds(this.regenStartTime);

            while (this.currentStamina < this.maxStamina)
            {
                // Debug.Log("regenning stamina");
                this.currentStamina += this.regenRateAmount;
                this.staminaBar.SetCurrent(this.currentStamina);
                yield return new WaitForSeconds(this.regenRate);
            }
            this.isRegen = null;
        }

        public void IncreaseMaxStamina(int amount)
        {
            if (isClient)
            {
                if (hasAuthority)
                {
                    IncreaseMaxStaminaWithAuthority(amount);
                }
                else
                {
                    Debug.LogWarning("ran IncreaseMaxStamina from client without authority. call this function from server instead.");
                }
            }
            else if (isServer)
            {
                RpcIncreaseMaxStamina(amount);
            }
        }

        private void IncreaseMaxStaminaWithAuthority(int amount)
        {
            if (!hasAuthority)
                return;
            this.maxStamina = maxStamina + amount;
            this.staminaBar.SetMax(maxStamina);
        }
        #endregion

        #region Movement

        public void IncreaseMovementSpeed(float amount)
        {
            if (isClient)
            {
                if (hasAuthority)
                {
                    IncreaseMovementSpeedWithAuthority(amount);
                }
                else
                {
                    Debug.LogWarning("ran IncreaseMovementSpeed from client without authority. call this function from server instead.");
                }
            }
            else if (isServer)
            {
                RpcIncreaseMovementSpeed(amount);
            }
        }

        private void IncreaseMovementSpeedWithAuthority(float amount)
        {
            if (!hasAuthority)
                return;
            movementSpeed += amount;
        }

        public void IncreaseDodgeSpeed(float amount)
        {
            if (isClient)
            {
                if (hasAuthority)
                {
                    IncreaseDodgeSpeedWithAuthority(amount);
                }
                else
                {
                    Debug.LogWarning("ran IncreaseDodgeSpeed from client without authority. call this function from server instead.");
                }
            }
            else if (isServer)
            {
                RpcIncreaseDodgeSpeed(amount);
            }
        }

        private void IncreaseDodgeSpeedWithAuthority(float amount)
        {
            if (!hasAuthority)
                return;
            dodgeSpeed += amount;
        }

        public void IncreaseJumpHeight(float amount)
        {
            if (isClient)
            {
                if (hasAuthority)
                {
                    IncreaseJumpHeightWithAuthority(amount);
                }
                else
                {
                    Debug.LogWarning("ran IncreaseJumpHeight from client without authority. call this function from server instead.");
                }
            }
            else if (isServer)
            {
                RpcIncreaseJumpHeight(amount);
            }
        }

        private void IncreaseJumpHeightWithAuthority(float amount)
        {
            if (!hasAuthority)
                return;
            jumpHeight += amount;
        }

        #endregion

        #region fallDamage

        public void IncreaseFallDamageReduction(int amount)
        {
            if (isClient)
            {
                if (hasAuthority)
                {
                    IncreaseFallDamageReductionWithAuthority(amount);
                }
                else
                {
                    Debug.LogWarning("ran IncreaseFallDamageReduction from client without authority. call this function from server instead.");
                }
            }
            else if (isServer)
            {
                RpcIncreaseFallDamageReduction(amount);
            }
        }

        private void IncreaseFallDamageReductionWithAuthority(int amount)
        {
            if (!hasAuthority)
                return;

            fallDamageReduction += amount;
        }
        #endregion

        #region Getters

        public int GetCurrentStamina()
        {
            return this.currentStamina;
        }

        public float GetMovementSpeed()
        {
            return this.movementSpeed;
        }

        public float GetDashModifier()
        {
            return this.dashModifier;
        }

        public float GetJumpHeight()
        {
            return this.jumpHeight;
        }

        public float GetSpeedChangeRate()
        {
            return this.speedChangeRate;
        }

        public float GetDodgeSpeed()
        {
            return this.dodgeSpeed;
        }

        public float GetDodgeTime()
        {
            return this.dodgeTime;
        }

        public int GetDodgeCost()
        {
            return this.dodgeCost;
        }

        public bool IsHungry()
        {
            return this.isHungry;
        }

        public bool IsAlive()
        {
            return this.isAlive;
        }

        #endregion

        #region Cmds
        // [Command] is for a CLIENT telling the SERVER to run this method.

        [Command]
        private void CmdIncreaseJumpHeight(float amount)
        {
            RpcIncreaseJumpHeight(amount);
        }

        [Command]
        private void CmdIncreaseDodgeSpeed(float amount)
        {
            RpcIncreaseDodgeSpeed(amount);
        }

        [Command]
        private void CmdIncreaseMovementSpeed(float amount)
        {
            RpcIncreaseMovementSpeed(amount);
        }

        [Command]
        private void CmdIncreaseFallDamageReduction(int amount)
        {
            RpcIncreaseFallDamageReduction(amount);
        }

        [Command]
        private void CmdFasterRegenRate(float amount)
        {
            RpcFasterRegenRate(amount);
        }

        [Command]
        private void CmdIncreaseMaxStamina(int amount)
        {
            RpcIncreaseMaxStamina(amount);
        }

        [Command]
        private void CmdMaxHealthChanged(int newValue)
        {
            RpcMaxHealthChangedWithAuthority(newValue);
        }

        [Command]
        private void CmdHealthChanged(int newValue)
        {
            changeHealthOnServer(newValue);
        }

        [Command]
        private void CmdKilled()
        {
            RpcKilled();
        }
        #endregion

        #region Rpcs
        // [ClientRpc] is for a SERVER telling ALL CLIENTS to run this method.

        [ClientRpc]
        private void RpcChangeHealthWithAuthority(int newCurrentHealth)
        {
            this.currentHealth = newCurrentHealth;
            if (hasAuthority)
            {
                this.healthBar.SetCurrent(newCurrentHealth);
            }
            else
            {

            }
        }

        [ClientRpc]
        private void RpcIncreaseJumpHeight(float amount)
        {
            if (hasAuthority)
            {
                IncreaseJumpHeightWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcIncreaseDodgeSpeed(float amount)
        {
            if (hasAuthority)
            {
                IncreaseDodgeSpeedWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcIncreaseMovementSpeed(float amount)
        {
            if (hasAuthority)
            {
                IncreaseMovementSpeedWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcIncreaseFallDamageReduction(int amount)
        {
            if (hasAuthority)
            {
                IncreaseFallDamageReductionWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcFasterRegenRate(float amount)
        {
            if (hasAuthority)
            {
                FasterRegenRateWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcIncreaseMaxStamina(int amount)
        {
            if (hasAuthority)
            {
                IncreaseMaxStaminaWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcRestoreStamina(int amount)
        {
            if (hasAuthority)
            {
                RestoreStaminaWithAuthority(amount);
            }
        }

        [ClientRpc]
        private void RpcMaxHealthChangedWithAuthority(int newValue)
        {
            this.maxHealth = newValue;
            if (hasAuthority)
            {
                this.healthBar.SetMax(newValue);
            }
            else
            {

            }
        }

        [ClientRpc]
        private void RpcHealthChanged(int newValue)
        {
            RpcChangeHealthWithAuthority(newValue);
        }

        [ClientRpc]
        private void RpcKilled()
        {
            this.isAlive = false;
        }

        #endregion

    }
}