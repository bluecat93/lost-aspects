using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object;
using UnityEngine.Events;
using Animation;


namespace Player
{
    public class Abilities : MonoBehaviour
    {
        private int maximumNumberOfAttacks = 3;

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

        private EventsHandler _eventHandler;

        private EventsHandler EvntHndlr
        {
            get
            {
                if (this._eventHandler == null)
                    this._eventHandler = GetComponentInChildren<EventsHandler>();
                return this._eventHandler;
            }
        }

        [HideInInspector] public int attackNumber;
        [HideInInspector] public UnityEvent<int> attackEvent;

        // Start is called before the first frame update
        void Start()
        {
            attackNumber = 1;
            this.EvntHndlr.OnAttackEventTrigger.AddListener(AttackEvent);
        }

        // Update is called once per frame
        void Update()
        {
            // float attacking = Input.GetAxisRaw("Swing");
            if (Input.GetButtonDown("Swing") && !this.WpnAttck.isAttacking)
            {
                this.Anmtor.SetInteger("Attack Number", attackNumber);
                this.Anmtor.SetTrigger("Attack");
            }
        }

        public void AttackEvent(bool isAttacking)
        {
            if (isAttacking)
            {
                this.WpnAttck.isAttacking = true;

            }
            else
            {
                this.WpnAttck.isAttacking = false;
                attackNumber = attackNumber == maximumNumberOfAttacks ? 1 : attackNumber + 1;
            }
        }

    }

}