using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunch : MonoBehaviour
{
    public EnemyAi enemyAi;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && enemyAi.isAttacking && enemyAi.attackOnlyOnce)
        {
            enemyAi.playerStats.TakeDamage(enemyAi.attackDamage);
            enemyAi.attackOnlyOnce = false;
        }
    }
}
