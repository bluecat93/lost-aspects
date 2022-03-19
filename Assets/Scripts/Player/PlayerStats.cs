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
    public bool isAlive = true;

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
        currentHealth = maxHealth;
        healthBar.SetMax(maxHealth);
        isAlive = true;

        currentHunger = maxHunger;
        hungerBar.SetMax(maxHunger);

        currentStamina = maxStamina;
        staminaBar.SetMax(maxStamina);

        InvokeRepeating("GettingHungry", 5.0f, 10.0f);
        InvokeRepeating("CheckHunger", 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // Button to test death
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentHealth = 0;
        }
        if (currentHealth <= 0)
        {
            isAlive = false;

            gameOverUI.SetActive(true);
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
        if (currentHunger > 0)
        {
            currentHunger--;
            hungerBar.SetCurrent(currentHunger);
        }
    }

    void CheckHunger()
    {
        if (currentHunger == 0) // how we take damage.
        {
            TakeDamage(1, false);
        }
    }

    public void Respawn()
    {
        gameOverUI.SetActive(false);
        respawn = FindObjectOfType<RespawnScript>();
        respawn.RespawnPlayer();
        Heal();
        Eat();
        thirdPersonCamera.SetActive(true);
        isAlive = true;
        Cursor.lockState = CursorLockMode.Locked; // locking cursor to not show it while moving.
    }
    public void Heal()
    {
        currentHealth = maxHealth;
        healthBar.SetCurrent(maxHealth);
    }
    public void Eat()
    {
        currentHunger = maxHunger;
        hungerBar.SetCurrent(maxHunger);
    }

    public void ChangeStamina(int amount)
    {
        currentStamina -= amount;
        currentStamina = currentStamina > maxStamina ? maxStamina : currentStamina;
        staminaBar.SetCurrent(currentStamina);
    }
}
