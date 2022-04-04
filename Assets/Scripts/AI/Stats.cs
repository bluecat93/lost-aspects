using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Stats : MonoBehaviour
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
        [Tooltip("The time (in seconds) it takes between each attack")]
        [SerializeField] private float attackTimeIntervals = 1f;

        [Header("Distance")]
        [Tooltip("The maximum distance it takes for the enemy to stop following the character")]
        [SerializeField] private float maxFollowDistance = 20f;
        [Tooltip("The minimum distance it takes for the enemy to spot the character")]
        [SerializeField] private float sightDistance = 10f;
        [Tooltip("The minimum distance it takes for the enemy to start the attack animation on the character")]
        [SerializeField] private float attackDistance = 5f;

        private int currentHealth;
        private float invulnerabilityFrame = 0f;

        // Start is called before the first frame update
        void Start()
        {
            this.currentHealth = this.maxHealth;
        }

        public bool isEnemyAlive()
        {
            return this.currentHealth >= 0;
        }
        public void TakeDamage(int damage)
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
            if (this.currentHealth > this.maxHealth)
                this.currentHealth = this.maxHealth;
        }

        public float getDeathTimer()
        {
            return this.deathTimer;
        }
        public float getMaxFollowDistance()
        {
            return this.maxFollowDistance;
        }
        public float getSightDistance()
        {
            return this.sightDistance;
        }
        public float getAttackDistance()
        {
            return this.attackDistance;
        }
        public float getAttackTimeIntervals()
        {
            return this.attackTimeIntervals;
        }
        public int getAttackDamage()
        {
            return this.attackDamage;
        }


    }

}
