using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{

    /// <summary>
    /// Class <c>MeleeAttack</c> create a melee attack for the enemy ai. Note: must have a collider on the same component as this.
    /// </summary>
    public class MeleeAttack : Attack
    {
        private bool attackOnlyOnce;
        private bool isAttacking;

        private void Start()
        {
            EnmyAi.attackEvent.AddListener(AttackEvent);
            this.attackOnlyOnce = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player" && this.attackOnlyOnce && isAttacking)
            {
                this.EnmyAi.getPlayerStats().TakeDamage((int)(this.EnmyStts.GetAttackDamage() * damageModifier));
                this.attackOnlyOnce = false;
            }
        }

        public override void AttackEvent(int attackNumber, bool isStartAttack)
        {
            isAttacking = isStartAttack && this.attackNumber == attackNumber;
            //Debug.Log("isAttacking = " + isAttacking);
            if (isAttacking)
            {
                this.attackOnlyOnce = true;
            }
        }
    }

}
