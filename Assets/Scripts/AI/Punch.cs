using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
namespace Enemy
{
    public class Punch : MonoBehaviour
    {
        private bool isAttacking = false;
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


        private void start()
        {
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Player" && this.EnmyAi.getIsAttacking() && this.EnmyAi.getAttackOnlyOnce())
            {
                this.EnmyAi.getPlayerStats().TakeDamage(this.EnmyStts.getAttackDamage());
                this.EnmyAi.setAttackOnlyOnce(false);
            }
        }
    }

}
