using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    abstract public class Attack : MonoBehaviour
    {
        [Header("Attacking and Damage")]
        [Tooltip("the number of the attack in the combo. if its a single attack leave it at 1")]
        [SerializeField] protected int attackNumber = 1;
        [Tooltip("The attack damage modifier")]
        [SerializeField] protected float damageModifier = 1f;

        private Stats _enemyStats;
        protected Stats EnmyStts
        {
            get
            {
                if (this._enemyStats == null)
                    this._enemyStats = GetComponentInParent<Stats>();

                return this._enemyStats;
            }
        }

        private Ai _enemyAi;
        protected Ai EnmyAi
        {
            get
            {
                if (this._enemyAi == null)
                    this._enemyAi = GetComponentInParent<Ai>();

                return this._enemyAi;
            }
        }

        abstract public void AttackEvent(int attackNumber, bool isStartAttack);



    }

}
