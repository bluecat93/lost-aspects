using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private int weaponBaseAttackDamage = 0;
    public int weaponAttackDamage = 10;
    public bool isAttacking = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && isAttacking)
        {
            //enemy was hit
            Transform enemy = other.gameObject.transform;
            EnemyAi enemyAi = enemy.GetComponent<EnemyAi>();
            enemyAi.TakeDamage(weaponBaseAttackDamage + weaponAttackDamage);
        }
    }
}
