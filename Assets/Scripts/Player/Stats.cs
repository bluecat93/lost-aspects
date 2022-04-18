using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsUpDisplay;

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
    public class Stats : MonoBehaviour
    {
        [Header("Health Variables")]
        [Tooltip("Health character starts with")]
        [SerializeField] private int maxHealth = 100;
        [Tooltip("Health character currently has")]
        [SerializeField] private int currentHealth;
        private bool isAlive = true;

        [Header("Stamina Variables")]
        [Tooltip("Stamina character starts with")]
        [SerializeField] private int maxStamina = 100;
        [Tooltip("Stamina character currently has")]
        [SerializeField] private int currentStamina;

        [Header("Stamina Recharge Variables")]
        [Tooltip("When to start regenerating stamina")]
        [SerializeField] private float regenStartTime = 2f;
        [Tooltip("How fast stamina recharges")]
        [SerializeField] private int regenRate = 1;
        [Tooltip("How often stamina recharges, lower is better")]
        [SerializeField] private float regenTick = 0.1f;
        [Tooltip("How often stamina recharges when hungry")]
        [SerializeField] private float regenWhenHungry = 0.5f;

        // Check if currently regenning
        private Coroutine isRegen;
        // Variables to hold new and hold regen rates when hungry or full
        private float regenRateWhenFull = 1f;
        // private float currentRegenRate = 1f;

        [Header("Basic Movement Variables")]
        [Tooltip("The character's base movement speed")]
        [SerializeField] private float movementSpeed = 5.0f;
        [Tooltip("Acceleration and deceleration")]
        [SerializeField] private float speedChangeRate = 10.0f;

        [Header("Dashing Variables")]
        [Tooltip("The speed multiplier when sprinting")]
        [SerializeField] private float dashModifier = 1.5f;

        [Header("Jumping Variables")]
        [Tooltip("The higher this field, the higher the jump")]
        [SerializeField] private float jumpHeight = 5f;

        [Header("Dodging Variables")]
        [Tooltip("The speed of the character while dodgeing")]
        [SerializeField] private float dodgeSpeed = 10f;
        [Tooltip("The time (in seconds) it takes for the character to dodge (from start to finish)")]
        [SerializeField] private float dodgeTime = 0.5f;
        [Tooltip("How much stamina a roll takes")]
        [SerializeField] private int dodgeCost = -50;

        [Header("Hunger Variables")]
        [Tooltip("Hunger when character is full")]
        [SerializeField] private int maxHunger = 100;
        [Tooltip("Characters current hunger rating")]
        [SerializeField] private int currentHunger;
        [Tooltip("Start lowering hunger")]
        [SerializeField] private float hungerStartTime = 5.0f;
        [Tooltip("How often to lower hunger")]
        [SerializeField] private float hungerRepeatTime = 10.0f;

        private bool isHungry = false;

        [Header("UI Bars")]
        [Tooltip("Bar that displays character health")]
        [SerializeField] private BarScript healthBar;
        [Tooltip("Bar that displays character hunger")]
        [SerializeField] private BarScript hungerBar;
        [Tooltip("Bar that displays character stamina")]
        [SerializeField] private BarScript staminaBar;

        [Header("Game objects")]
        [Tooltip("Game over screen")]
        [SerializeField] private GameObject gameOverUI;
        [Tooltip("Camera that follows player")]
        [SerializeField] private GameObject thirdPersonCamera;

        [Header("Other variables")]
        [Tooltip("Reduce fall damage")]
        [SerializeField] private int fallDamageReduction = 0;


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

        // Start is called before the first frame update
        void Start()
        {
            this.currentHealth = this.maxHealth;
            this.healthBar.SetMax(this.maxHealth);
            this.isAlive = true;

            this.currentHunger = this.maxHunger;
            this.hungerBar.SetMax(this.maxHunger);

            this.currentStamina = this.maxStamina;
            this.staminaBar.SetMax(this.maxStamina);

            regenRateWhenFull = regenTick;

            InvokeRepeating("GettingHungry", this.hungerStartTime, this.hungerRepeatTime);
            InvokeRepeating("CheckHunger", 0.5f, 0.5f);
        }

        // Update is called once per frame
        void Update()
        {
            // Button to test death
            if (Input.GetKeyDown(KeyCode.P))
            {
                this.currentHealth = 0;
            }
            if (this.currentHealth <= 0)
            {
                this.isAlive = false;

                this.gameOverUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }

        }

        public void TakeDamage(int damage, bool isBlockable = true)
        {
            if (this.currentHealth > 0 && ((isBlockable && !this.ThrdPrsonMvmt.IsRolling) || (!isBlockable)))
            {
                this.currentHealth -= damage;
                this.healthBar.SetCurrent(currentHealth);
            }
        }

        public void TakePercentileDamage(int damage, bool isBlockable = true)
        {
            this.TakeDamage(damage * 100 / this.maxHealth, isBlockable);
        }

        void GettingHungry()
        {
            if (this.currentHunger > 0)
            {
                this.currentHunger--;
                this.hungerBar.SetCurrent(this.currentHunger);
            }
        }

        void CheckHunger()
        {
            if (this.currentHunger == 0) // how we know if character is hungry
            {
                this.isHungry = true;
                regenTick = regenWhenHungry;
            }
            else
            {
                this.isHungry = false;
                regenTick = regenRateWhenFull;
            }
        }

        public void Respawn()
        {
            this.gameOverUI.SetActive(false);
            this.respawn = FindObjectOfType<RespawnScript>();
            this.respawn.RespawnPlayer();
            Heal();
            Eat();
            this.thirdPersonCamera.SetActive(true);
            this.isAlive = true;
            Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
        }
        public void Heal()
        {
            this.currentHealth = this.maxHealth;
            this.healthBar.SetCurrent(this.maxHealth);
        }
        public void Eat()
        {
            this.currentHunger = this.maxHunger;
            this.hungerBar.SetCurrent(this.maxHunger);
        }

        public void ChangeStamina(int amount)
        {
            this.currentStamina += amount;
            this.currentStamina = this.currentStamina > this.maxStamina ? this.maxStamina : this.currentStamina;
            this.staminaBar.SetCurrent(this.currentStamina);
            if (this.currentStamina < this.maxStamina)
            {
                if (this.isRegen != null)
                    StopCoroutine(this.isRegen);
                this.isRegen = StartCoroutine(RegenStamina());
            }
        }

        private IEnumerator RegenStamina()
        {
            // Debug.Log("regenning stamina");
            // Start regenerating stamina after a few seconds
            yield return new WaitForSeconds(this.regenStartTime);

            while (this.currentStamina < this.maxStamina)
            {
                // Debug.Log("regenning stamina");
                this.currentStamina += this.regenRate;
                this.staminaBar.SetCurrent(this.currentStamina);
                yield return new WaitForSeconds(this.regenTick);
            }
            this.isRegen = null;
        }

        #region Getters and Setters

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

        public void SetMovementSpeed(float movementSpeed)
        {
            this.movementSpeed = movementSpeed;
        }

        #endregion


        // public void RechargeStamina()
        // {
        //     if (this.currentStamina != this.maxStamina)
        //         ChangeStamina(RechargeRate);
        // }
    }
}