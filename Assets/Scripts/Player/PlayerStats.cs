using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int maxHunger = 100;
    public int currentHunger;
    public int maxStamina = 100;
    public int currentStamina;
    public int fallDamageReduction = 0;


    public BarScript healthBar;
    public BarScript hungerBar;
    public BarScript staminaBar;
    public GameObject gameOverUI;
    public GameObject thirdPersonCamera;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isHungry = false;

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

        InvokeRepeating("GettingHungry", 5.0f, 10.0f);
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
        if (this.currentHunger == 0) // how we take damage.
        {
            isHungry = true;
        }
        else
        {
            isHungry = false;
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
        this.currentStamina -= amount;
        this.currentStamina = this.currentStamina > this.maxStamina ? this.maxStamina : this.currentStamina;
        this.staminaBar.SetCurrent(this.currentStamina);
    }
}
