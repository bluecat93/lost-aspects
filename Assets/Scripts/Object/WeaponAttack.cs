using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Object
{
    public class WeaponAttack : MonoBehaviour
    {
        private int weaponBaseAttackDamage = 0;
        public int weaponAttackDamage = 10;
        [HideInInspector] public bool isAttacking = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy" && this.isAttacking)
            {
                //enemy was hit
                Transform enemy = other.gameObject.transform;
                Enemy.Stats enemyStats = enemy.GetComponent<Enemy.Stats>();
                enemyStats.TakeDamage(this.weaponBaseAttackDamage + this.weaponAttackDamage);
            }
        }
    }

}