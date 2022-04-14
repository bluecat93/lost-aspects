using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object;

namespace Player
{
    public class Abilities : MonoBehaviour
    {
        private WeaponAttack _weaponAttack;

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

        [HideInInspector] public int attackNumber;

        // Start is called before the first frame update
        void Start()
        {
            attackNumber = 0;
        }

        // Update is called once per frame
        void Update()
        {
            // float attacking = Input.GetAxisRaw("Swing");
            if (Input.GetButtonDown("Swing") && !this.WpnAttck.isAttacking)
            {
                attackNumber++;
                if (attackNumber > 3)
                    attackNumber = 1;
                this.Anmtor.SetInteger("Attack Number", attackNumber);
                this.Anmtor.SetTrigger("Attack");
            }
            this.WpnAttck.isAttacking = this.Anmtor.GetCurrentAnimatorStateInfo(0).IsName("Three hit combo 1") ||
             this.Anmtor.GetCurrentAnimatorStateInfo(0).IsName("Three hit combo 2") ||
              this.Anmtor.GetCurrentAnimatorStateInfo(0).IsName("Three hit combo 3");
        }

    }

}