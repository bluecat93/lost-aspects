using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
namespace Enemy
{
    public class Stats : NetworkBehaviour
    {
        [Header("Health and Survivability")]
        [Tooltip("The maximum health of the enemy")]
        [SerializeField] private int maxHealth = 100;
        [Tooltip("The time (in seconds) it takes the enemy to take damage between each damage type")]
        [SerializeField] private float invulnerabilityFrameAmount = 0.5f;
        [Tooltip("The amount of time it takes the enemy corpse to disappear (starting from the start of the death animation")]
        [SerializeField] private float deathTimer = 5f;

        [Header("Attack and Damage")]
        [Tooltip("The basic damage this enemy deals with each attack.")]
        [SerializeField] private int attackDamage = 5;
        [Tooltip("The numbers of attacks per combo")]
        [SerializeField] private int numberOfAttacks = 1;

        [Header("Distance")]
        [Tooltip("The maximum distance it takes for the enemy to stop following the character")]
        [SerializeField] private float maxFollowDistance = 20f;
        [Tooltip("The minimum distance it takes for the enemy to spot the character")]
        [SerializeField] private float sightDistance = 10f;
        [Tooltip("The minimum distance it takes for the enemy to start the attack animation on the character")]
        [SerializeField] private float attackDistance = 5f;

        [SyncVar(hook = nameof(LogCheck))] private int currentHealth;
        private float invulnerabilityFrame = 0f;

        // Start is called before the first frame update
        void Start()
        {
            this.currentHealth = this.maxHealth;
        }
        private void LogCheck(int newValue, int oldValue)
        {
            if (isClient)
            {
                Debug.Log("this is client, enemy had: " + oldValue + " health\nnow he has: " + newValue + " health\nhas authority?: " + hasAuthority);
            }
            if (isServer)
            {
                Debug.Log("i though this only happens on client wtf???");
            }
        }
        public bool isEnemyAlive()
        {
            return this.currentHealth >= 0;
        }
        public void TakeDamage(int damage)
        {
            if (isServer)
            {
                TakeDamageFromServer(damage);
            }
            if (isClient)
            {
                if (hasAuthority)
                {
                    CmdTakeDamge(damage);
                }
                else
                {
                    // the stats will change by the attacking player anyway so you can do nothing here.
                }
            }
        }

        [Command]
        private void CmdTakeDamge(int damage)
        {
            TakeDamageFromServer(damage);
        }
        private void TakeDamageFromServer(int damage)
        {
            if (this.invulnerabilityFrame < Time.time)
            {
                this.invulnerabilityFrame = Time.time + this.invulnerabilityFrameAmount;
                this.currentHealth -= damage;
            }
        }
        public void Heal(int heal)
        {
            this.currentHealth += heal;
            this.currentHealth = this.currentHealth > this.maxHealth ? this.maxHealth : this.currentHealth;
        }

        public float GetDeathTimer()
        {
            return this.deathTimer;
        }
        public float GetMaxFollowDistance()
        {
            return this.maxFollowDistance;
        }
        public float GetSightDistance()
        {
            return this.sightDistance;
        }
        public float GetAttackDistance()
        {
            return this.attackDistance;
        }
        public int GetAttackDamage()
        {
            return this.attackDamage;
        }

        public int GetNumberOfAttacks()
        {
            return this.numberOfAttacks;
        }

        public int GetMaxHealth()
        {
            return this.maxHealth;
        }

    }

}
