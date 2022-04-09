using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object;

namespace Player
{
    public class Abilities : MonoBehaviour
    {
        public float maxAttackDistance = 1.0f;

        private Ray[] ray;

        public float playerMiddleHeight = 2;

        public WeaponAttack _weaponAttack;

        private WeaponAttack WpnAttck
        {
            get
            {
                if (this._weaponAttack == null)
                    this._weaponAttack = GetComponentInChildren<WeaponAttack>();

                return this._weaponAttack;
            }
        }

        private Animator _animator;

        private Animator Anmtor
        {
            get
            {
                if (this._animator == null)
                    this._animator = GetComponentInChildren<Animator>();

                return this._animator;
            }
        }

        private Stats _playerStats;

        private Stats PlyrStts
        {
            get
            {
                if (this._playerStats == null)
                    this._playerStats = GetComponent<Stats>();

                return this._playerStats;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            ray = new Ray[3];
        }

        // Update is called once per frame
        void Update()
        {
            // TODO: add the content of this for loop to the for loop inside the raycast
            // when you no longer want to see the raycast at all times.
            // 3 rays that indicate the attack range.
            for (int i = -1; i <= 1; i++)
            {
                ray[i + 1] = new Ray(transform.position + Vector3.up * this.playerMiddleHeight,
                    (transform.forward + (i * transform.right)).normalized * this.maxAttackDistance);
                Debug.DrawRay(ray[i + 1].origin, ray[i + 1].direction * this.maxAttackDistance);
            }

            // float attacking = Input.GetAxisRaw("Swing");
            if (Input.GetButtonDown("Swing"))
            {
                this.Anmtor.SetTrigger("Attack");
            }
            this.WpnAttck.isAttacking = this.Anmtor.GetCurrentAnimatorStateInfo(0).IsName("Attack");
        }

        // still saved this method. maybe someone will want to see how to find the closest object from a given list of objects.
        [Obsolete("This method is deprecated, please use checkHit instead.")]
        private Transform checkHitDeprecated()
        {
            Transform closestViableEnemy = null;
            float closestViableEnemyDistance = maxAttackDistance;
            /*        foreach (Transform enemy in EnemiesList.getEnemies())
                    {
                        float distance = Vector3.Distance(transform.position, enemy.position);
                        if (closestViableEnemyDistance >= distance)
                        {
                            closestViableEnemyDistance = distance;
                            closestViableEnemy = enemy;
                        }
                    }*/
            return closestViableEnemy;
        }


    }

}