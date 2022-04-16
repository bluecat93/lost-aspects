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
        [HideInInspector] public bool isAlive = true;

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
        [SerializeField] public float movementSpeed = 5.0f;
        [Tooltip("Acceleration and deceleration")]
        [SerializeField] public float speedChangeRate = 10.0f;

        [Header("Dashing Variables")]
        [Tooltip("The speed multiplier when sprinting")]
        [SerializeField] public float dashModifier = 1.5f;

        [Header("Jumping Variables")]
        [Tooltip("The higher this field, the higher the jump")]
        [SerializeField] public float jumpHeight = 5f;

        [Header("Dodging Variables")]
        [Tooltip("The speed of the character while dodgeing")]
        [SerializeField] public float dodgeSpeed = 10f;
        [Tooltip("The time (in seconds) it takes for the character to dodge (from start to finish)")]
        [SerializeField] public float dodgeTime = 0.5f;
        [Tooltip("How much stamina a roll takes")]
        [SerializeField] public int dodgeCost = -50;

        [Header("Hunger Variables")]
        [Tooltip("Hunger when character is full")]
        public int maxHunger = 100;
        [Tooltip("Characters current hunger rating")]
        public int currentHunger;
        [Tooltip("Start lowering hunger")]
        public float hungerStartTime = 5.0f;
        [Tooltip("How often to lower hunger")]
        public float hungerRepeatTime = 10.0f;

        [HideInInspector] public bool isHungry = false;

        [Header("UI Bars")]
        [Tooltip("Bar that displays character health")]
        public BarScript healthBar;
        [Tooltip("Bar that displays character hunger")]
        public BarScript hungerBar;
        [Tooltip("Bar that displays character stamina")]
        public BarScript staminaBar;

        [Header("Game objects")]
        [Tooltip("Game over screen")]
        public GameObject gameOverUI;
        [Tooltip("Camera that follows player")]
        public GameObject thirdPersonCamera;

        [Header("Other variables")]
        [Tooltip("Reduce fall damage")]
        public int fallDamageReduction = 0;


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
            Debug.Log("regenning stamina");
            // Start regenerating stamina after a few seconds
            yield return new WaitForSeconds(this.regenStartTime);

            while (this.currentStamina < this.maxStamina)
            {
                Debug.Log("regenning stamina");
                this.currentStamina += this.regenRate;
                this.staminaBar.SetCurrent(this.currentStamina);
                yield return new WaitForSeconds(this.regenTick);
            }
            this.isRegen = null;
        }

        public int GetCurrentStamina()
        {
            return this.currentStamina;
        }

        // public void RechargeStamina()
        // {
        //     if (this.currentStamina != this.maxStamina)
        //         ChangeStamina(RechargeRate);
        // }
    }
}