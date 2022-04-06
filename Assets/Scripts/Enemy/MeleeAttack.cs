using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{

    /// <summary>
    /// Class <c>MeleeAttack</c> create a melee attack for the enemy ai. Note: must have a collider on the same component as this.
    /// </summary>
    public class MeleeAttack : MonoBehaviour
    {
        private Ai _enemyAi;
        private Ai EnmyAi
        {
            get
            {
                if (this._enemyAi == null)
                    this._enemyAi = GetComponentInParent<Ai>();

                return this._enemyAi;
            }
        }

        private Stats _enemyStats;
        private Stats EnmyStts
        {
            get
            {
                if (this._enemyStats == null)
                    this._enemyStats = GetComponentInParent<Stats>();

                return this._enemyStats;
            }
        }

        private bool attackOnlyOnce;
        [Header("Attacking and Damage")]
        [Tooltip("the number of the attack in the combo. if its a single attack leave it at 1")]
        [SerializeField] private int attackNumber = 1;
        [Tooltip("The attack modifier")]
        [SerializeField] private float damageModifier = 1f;
        private bool isAttacking;

        private void Start()
        {
            EnmyAi.attackEvent.AddListener(AttackEvent);
            this.attackOnlyOnce = false;
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player" && this.attackOnlyOnce)
            {
                this.EnmyAi.getPlayerStats().TakeDamage((int)(this.EnmyStts.getAttackDamage() * damageModifier));
                this.attackOnlyOnce = false;
            }
        }
        public void AttackEvent(int attackNumber, bool isStartAttack)
        {
            isAttacking = isStartAttack && this.attackNumber == attackNumber;
            // Debug.Log("isAttacking = " + isAttacking);
            if (isAttacking)
            {
                this.attackOnlyOnce = true;
            }
        }
    }

}
