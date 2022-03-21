using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPunch : MonoBehaviour
{
    private EnemyAi ai;
    private EnemyStats stats;
    private void Start()
    {
        ai = GetComponentInParent<EnemyAi>();
        stats = GetComponentInParent<EnemyStats>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && ai.GetIsAttacking() && ai.GetAttackOnlyOnce())
        {
            ai.GetPlayerStats().TakeDamage(stats.getAttackDamage());
            ai.SetAttackOnlyOnce(false);
        }
    }
}
