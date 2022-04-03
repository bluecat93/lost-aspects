using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunch : MonoBehaviour
{
    private EnemyAi _enemyAi;

    private EnemyAi EnmyAi
    {
        get
        {
            if (this._enemyAi == null)
                this._enemyAi = GetComponentInParent<EnemyAi>();

            return this._enemyAi;
        }
    }
    private EnemyStats _enemyStats;

    private EnemyStats EnmyStts
    {
        get
        {
            if (this._enemyStats == null)
                this._enemyStats = GetComponentInParent<EnemyStats>();

            return this._enemyStats;
        }
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
