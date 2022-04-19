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
        [Header("Animations")]
        [Tooltip("The default animator controller.")]
        [SerializeField] private RuntimeAnimatorController defaultAnimatorController;

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

        private EventsHandler _eventHandler;

        private bool isEquiped = false;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Equip"))
            {
                if (isEquiped)
                {
                    unequipWeapon();
                }
                else
                {
                    EquipWeapon();
                }
                isEquiped = !isEquiped;
            }
        }

        //Equip or unequip a weapon. NOTE: this will only work with one weapon type. if we will have more than we have to insantiate them in the right positions by getting the offsets and the like.
        #region Weapon Equpiment
        public void EquipWeapon()
        {
            WeaponAttack meleeWeapon = GetComponentInChildren<WeaponAttack>(true);
            meleeWeapon.gameObject.SetActive(true);
            meleeWeapon.Equip();
        }
        public void unequipWeapon()
        {
            WeaponAttack meleeWeapon = GetComponentInChildren<WeaponAttack>(true);
            meleeWeapon.gameObject.SetActive(false);
            Functions.SetAnimatorController(Anmtor, this.defaultAnimatorController);

        }

        #endregion

    }

}