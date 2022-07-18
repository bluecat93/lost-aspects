using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace Enemy
{
    public class Stats : NetworkBehaviour
    {
        [Serializable]
        public class ItemDrop
        {
            public int amount;
            public GameObject itemPrefab;
        }

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

        [Header("ItemDrop")]
        [SerializeField] private List<ItemDrop> itemDrops;

        [SyncVar(hook = nameof(LogCheck))] private int currentHealth;
        private float invulnerabilityFrame = 0f;




        // Start is called before the first frame update
        void Start()
        {
            this.currentHealth = this.maxHealth;
        }


        // NOTE: Was using it to test hook. saving this for later use of hook since it works.
        // This is how a hook is called: [SyncVar(hook = nameof(LogCheck))]
        // Still using it for testing the enemy health bar. 
        // To remove this dont forget to remove the hook in currentHealth syncvar by replaceing the above with [SyncVar]
        private void LogCheck(int oldValue, int newValue)
        {
            if (isClient)
            {
                if (oldValue != 0) // old value = 0 when an enemy is spawned (game starts)
                    Debug.Log("this is client, enemy had: " + oldValue + " health\nnow he has: " + newValue + " health");
            }
            if (isServer)
            {

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

        public void Knockbacked(float x, float y, float z)
        {
            if (isServer)
            {
                KnockbackedFromServer(x, y, z);
            }
            else if (isClient)
            {
                CmdKnockbacked(x, y, z);
            }
        }
        [Command]
        private void CmdKnockbacked(float x, float y, float z)
        {
            KnockbackedFromServer(x, y, z);
        }
        private void KnockbackedFromServer(float x, float y, float z)
        {
            Debug.Log("got to here with vector 3 = " + new Vector3(x, 0, z));
            GetComponent<Rigidbody>().AddForce(new Vector3(x, 0, z), ForceMode.VelocityChange);
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

        public List<ItemDrop> GetItemDrops()
        {
            return itemDrops;
        }

    }

}
