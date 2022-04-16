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
                int finalDamage = this.weaponBaseAttackDamage + this.weaponAttackDamage;
                switch (GetComponentInParent<Player.Abilities>().attackNumber)
                {
                    case 2:
                        finalDamage += 5;
                        break;
                    case 3:
                        finalDamage = 50;
                        break;
                    default:
                        break;
                }
                enemyStats.TakeDamage(this.weaponBaseAttackDamage + this.weaponAttackDamage);

                // Debug.Log("enemy hit with attack number: " + GetComponentInParent<Player.Abilities>().attackNumber);
            }
        }
    }

}