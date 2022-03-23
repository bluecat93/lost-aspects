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
        if (other.gameObject.tag == "Enemy" && this.isAttacking)
        {
            //enemy was hit
            Transform enemy = other.gameObject.transform;
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            enemyStats.TakeDamage(this.weaponBaseAttackDamage + this.weaponAttackDamage);
        }
    }
}
